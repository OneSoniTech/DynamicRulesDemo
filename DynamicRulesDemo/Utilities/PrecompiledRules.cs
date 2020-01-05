using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DynamicRulesDemo
{
    /// Author: Cole Francis, Architect
    /// The pre-compiled rules type
    /// https://www.psclistens.com/insight/blog/quickly-build-a-business-rules-engine-using-c-and-lambda-expression-trees/
    public static class PrecompiledRules
    {
        private static readonly Dictionary<string, object> Symbols = new Dictionary<string, object>() {
            // {"user", CoreSetting.User},
            //{"db", CoreSetting.DbContext},
            //{"odata", CoreSetting.ODataClient}
        };       

        public static bool Vaidate<T, TContext>(this T parameter, TContext ctx, string predicate,IMemoryCache memoryCache) where T : class
        {
            try
            {
                return ValidateCore<T, TContext>(predicate,memoryCache)(parameter, ctx);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public static bool Vaidate<T>(this T parameter, string predicate, IMemoryCache memoryCache) where T : class
        {
            try
            {
                return ValidateCore<T>(predicate,memoryCache)(parameter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public static Func<T, TContext, bool> ValidateCore<T, TContext>(string predicate, IMemoryCache memoryCache) where T : class
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (memoryCache.TryGetValue(predicate, out var cachedExpression) && cachedExpression is Func<T, TContext, bool> fun)
            {
                return fun;
            }
            ParameterExpression view = Expression.Parameter(typeof(TContext), "vw");
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            Expression<Func<T, TContext, bool>> lambda = (Expression<Func<T, TContext, bool>>)DynamicExpressionParser.ParseLambda(new ParameterExpression[] { x, view }, typeof(bool),
                predicate, Symbols);
            var compiled = lambda.Compile();
            memoryCache.Set(predicate, compiled, absoluteExpiration: DateTimeOffset.Now.AddHours(24));
            return compiled;
        }
        public static Func<T, bool> ValidateCore<T>(string predicate, IMemoryCache memoryCache) where T : class
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (memoryCache.TryGetValue(predicate, out var cachedExpression) && cachedExpression is Func<T, bool> fun)
            {
                return fun;
            }
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            Expression<Func<T, bool>> lambda = (Expression<Func<T, bool>>)DynamicExpressionParser.ParseLambda(new ParameterExpression[] { x }, typeof(bool),
                predicate, Symbols);
            var compiled = lambda.Compile();
            memoryCache.Set(predicate, compiled, absoluteExpiration: DateTimeOffset.Now.AddHours(24));
            return compiled;
        }

        public static void ApplyRule<T, TContext>(this T parameter, TContext ctx, string expression, IMemoryCache memoryCache)
            where T : class
            where TContext : class
        {
            ApplyRuleCore<T, TContext>(expression,memoryCache).DynamicInvoke(parameter, ctx);
        }
        public static Delegate ApplyRuleCore<T, TContext>(string expression, IMemoryCache memoryCache) where T : class
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (memoryCache.TryGetValue(expression, out var cachedExpression) && cachedExpression is Delegate fun)
            {
                return fun;
            }
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            ParameterExpression view = Expression.Parameter(typeof(TContext), "vw");
            var body = DynamicExpressionParser.ParseLambda(new[] { x, view }, null, expression, Symbols);
            var compiled = body.Compile();
            memoryCache.Set(expression, compiled, absoluteExpiration: DateTimeOffset.Now.AddHours(24));
            return compiled;
        }

        public static void ApplyRule<T>(this T parameter, string expression, IMemoryCache memoryCache) where T : class
        {
            ApplyRuleCore<T>(expression,memoryCache)?.DynamicInvoke(parameter);
        }
        public static Delegate ApplyRuleCore<T>(string expression, IMemoryCache memoryCache) where T : class
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));
            if (memoryCache.TryGetValue(expression, out var cachedExpression) && cachedExpression is Delegate fun)
            {
                return fun;
            }
            ParameterExpression x = Expression.Parameter(typeof(T), "x");
            var body = DynamicExpressionParser.ParseLambda(new[] { x }, null, expression, Symbols);

            var compiled = body.Compile();
            memoryCache.Set(expression, compiled, absoluteExpiration: DateTimeOffset.Now.AddHours(24));
            return compiled;
        }
    }
}