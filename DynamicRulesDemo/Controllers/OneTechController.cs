﻿using DynamicRulesDemo.Models.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRulesDemo.Controllers
{
    public class OneTechController: ControllerBase
    {
        protected readonly OneTechDbContext Db;
        private readonly IMemoryCache _cache;
        protected readonly ILogger<OneTechController> Log;
        public OneTechController(ILogger<OneTechController> logger, OneTechDbContext context,IMemoryCache cache)
        {
            Log = logger;
            Db = context;
            _cache = cache;
        }
    public void RunRules<TEntity>(string rulekey,TEntity entity) where TEntity:class
        {
            var rules = Db.Rules.Where(x => x.IsActive && x.RuleKey == rulekey).ToList();
            if (rules != null)
            {
                if (rules.Any())
                {
                    var logics2Apply = rules.Where(x =>
                        x.RuleNature == RuleNature.Assignment &&
                        (string.IsNullOrWhiteSpace(x.ValidationDefination) ||
                         entity.Vaidate(rulekey, x.ValidationDefination, _cache)));

                    foreach (var rule in logics2Apply)
                    {
                        entity.ApplyRule(this, rule.AssignmentDefination, _cache);
                    }
                    var failedvalidations = rules.Where(x =>
                        !string.IsNullOrWhiteSpace(x.ValidationDefination) &&
                        x.RuleNature == RuleNature.Validation &&
                        !entity.Vaidate(rulekey, x.ValidationDefination, _cache));
                    int errornum = 0;
                    foreach (var failedvalidation in failedvalidations)
                    {
                        ModelState.AddModelError("", $"{++errornum}) " + failedvalidation.FailedMessage);
                    }
                }
            }
        }
    }
}
