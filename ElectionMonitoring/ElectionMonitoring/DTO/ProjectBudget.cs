using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionMonitoring.DTO
{
    public class ProjectBudget
    {
        //public List<DTO.Budget> Budget {get; set;}
        public IEnumerable<Models.Budget> Budget { get; set; }
        public decimal TotalBudget
        {
            get
            {
                return Budget.Sum(b => b.Amount);
            }
        }
    }
}