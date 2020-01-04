using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using DynamicExpression = DynamicRulesDemo.DynamicExpression;

namespace DynamicRulesDemo
{
    public class Rule
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [MaxLength(150)]
        public string RuleKey { get; set; }

        public string Description { get; set; }
        public RuleNature RuleNature { get; set; } = RuleNature.Validation;
        public string ValidationDefination { get; set; }
        public string AssignmentDefination { get; set; }
        public string FailedMessage { get; set; }
        public string SuccessMessage { get; set; }
        public bool IsActive { get; set; }
        public int ExecutionOrder { get; set; }

        public Rule()
        {
            Id = Guid.NewGuid().ToString("D");
        }
    }
   
}