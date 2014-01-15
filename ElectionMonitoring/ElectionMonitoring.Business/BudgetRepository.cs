using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElectionMonitoring.Business
{
    using ElectionMonitoring.Models;
    using AutoMapper;

    public class BudgetRepository : IBudgetRepository
    {
        private Data.ElectionMonitoringEntities entities = new Data.ElectionMonitoringEntities();

        public IEnumerable<Models.Budget> GetBudgets()
        {
            var dataBudgets = entities.Budgets;
            Mapper.CreateMap<Data.Budget, Models.Budget>();
            var modelBudgets = Mapper.Map<IEnumerable<Data.Budget>, IEnumerable<Models.Budget>>(dataBudgets);
            return modelBudgets;
        }

        public Models.Budget GetBudget(int BudgetID)
        {
            var dataBudget = entities.Budgets.Where(d => d.BudgetID == BudgetID).FirstOrDefault();
            Mapper.CreateMap<Data.Budget, Models.Budget>();
            var modelBudget = Mapper.Map<Data.Budget, Models.Budget>(dataBudget);
            return modelBudget;
        }

        public Models.Budget CreateBudget(Models.Budget budget)
        {
            Mapper.CreateMap<Models.Budget, Data.Budget>();
            var dataBudget = Mapper.Map<Models.Budget, Data.Budget>(budget);
            entities.AddToBudgets(dataBudget);
            var newID = entities.SaveChanges();
            var dw = entities.Budgets.Where(d => d.BudgetID == newID).FirstOrDefault();
            var don = dw;
            Mapper.CreateMap<Data.Budget, Models.Budget>();
            budget = Mapper.Map<Data.Budget, Models.Budget>(dataBudget);
            return budget;
        }

        public bool UpdateBudget(Models.Budget budget)
        {
            var dataBudget = entities.Budgets.SingleOrDefault(d => d.BudgetID == budget.BudgetID);
            if (dataBudget != null)
            {
                dataBudget.BudgetItem = budget.BudgetItem;
                dataBudget.Amount = budget.Amount;
                dataBudget.ProjectID = budget.ProjectID;
                
                var updated = entities.SaveChanges();
                return (updated > 0);
            }
            return false;
        }

        public bool DeleteBudget(int BudgetID)
        {
            var dataBudget = entities.Budgets.SingleOrDefault(d => d.BudgetID == BudgetID);
            if (dataBudget != null)
            {
                entities.Budgets.DeleteObject(dataBudget);
                return true;
            }
            return false;
        }
    }
}
