using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicRulesDemo.Models
{
    public class ExpenseLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public ExpenseType ExpenseType { get; set; }
        public string EmployeeName { get; set; }
        public decimal Amount { get; set; }
        public bool IsApproved { get; set; } = true;
        public ObjectState EntityState { get; set; }
        public ExpenseLog()
        {
            Id = Guid.NewGuid().ToString("D");
        }
    }
}
