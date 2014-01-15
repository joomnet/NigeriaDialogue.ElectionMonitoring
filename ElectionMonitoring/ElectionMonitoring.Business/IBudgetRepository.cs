using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    public interface IBudgetRepository
    {
        IEnumerable<Models.Budget> GetBudgets();
        Models.Budget GetBudget(int BudgetID);
        Models.Budget CreateBudget(Models.Budget budget);
        bool UpdateBudget(Models.Budget budget);
        bool DeleteBudget(int BudgetID);
    }
}
