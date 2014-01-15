using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Models
{
    public class Budget
    {
        public int BudgetID { get; set; }
        public int ProjectID { get; set; }
        public string BudgetItem { get; set; }
        public decimal Amount { get; set; }
    }
}
