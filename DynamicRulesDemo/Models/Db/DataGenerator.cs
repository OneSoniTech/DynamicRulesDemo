using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DynamicRulesDemo.Models.Db
{
    class DataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new OneTechDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<OneTechDbContext>>()))
            {
                // Look for any board games.
                if (!context.Rules.Any())
                {
                    context.Rules.AddRange(
                        new Rule
                        {
                            RuleKey = "CoreValidation_ExpenseLog",
                            ValidationDefination = "(x.ExpenseType==\"Fooding\"&&x.Amount<=2500)||x.ExpenseType!=\"Fooding\"",
                            ExecutionOrder = 1,
                            Description = "Fooding Type Expense is not allowed to be paid more than INR 2500.",
                            FailedMessage = "You cannot pay Fooding More than INR 2500",
                            IsActive = true,
                            RuleNature = RuleNature.Validation
                        },
                        new Rule
                        {
                            RuleKey = "CoreValidation_ExpenseLog",
                            ValidationDefination = "(x.ExpenseType==\"Travelling\"&&x.Amount>10000)",
                            AssignmentDefination = "x.IsApproved=false",
                            ExecutionOrder = 1,
                            Description = "If Travelling Type Expense Amount is More than INR 10000 Expense would go for approval.",
                            IsActive = true,
                            RuleNature = RuleNature.Assignment
                        },
                        new Rule
                        {
                            RuleKey = "CoreValidation_ExpenseLog",
                            ValidationDefination = "x.Amount>0",
                            ExecutionOrder = 2,
                            FailedMessage = "Expense Amount should be always greater than zero.",
                            Description = "Stop user from passing zero amount expense",
                            IsActive = true,
                            RuleNature = RuleNature.Validation
                        },
                        new Rule
                        {
                            RuleKey = "CoreValidation_ExpenseLog",
                            ValidationDefination = "x.Amount<=100000",
                            ExecutionOrder = 3,
                            FailedMessage = "Expense Amount should not exceed INR 100000.",
                            Description = "Stop user from passing expense amount more than INR 100000",
                            IsActive = true,
                            RuleNature = RuleNature.Validation
                        },
                        new Rule
                        {
                            RuleKey = "CoreValidation_ExpenseLog",
                            ValidationDefination = "(vw.Expenses.Where(EmployeeName==x.EmployeeName).Sum(Amount)+x.Amount)<=50000",
                            ExecutionOrder = 4,
                            FailedMessage = "This Employee has exceeded maximum expenses allowed for Travelling(INR 50000).",
                            Description = "Stop user from passing expenses more than INR 50000 to single person for travelling",
                            IsActive = true,
                            RuleNature = RuleNature.Validation
                        });
                    context.SaveChanges();
                }
                if (!context.Expenses.Any())
                {
                    context.Expenses.AddRange(
                        new ExpenseLog
                        {
                            ExpenseType = ExpenseType.Fooding,
                            Amount = 2000,
                            EmployeeName = "Mukesh Rebari",
                            IsApproved = true
                        },
                        new ExpenseLog
                        {
                            ExpenseType = ExpenseType.Travelling,
                            Amount = 12000,
                            EmployeeName = "Saroj Rebari"
                        });
                    context.SaveChanges();
                }
            }
        }
    }
}
