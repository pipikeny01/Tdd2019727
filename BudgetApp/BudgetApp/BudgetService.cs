using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetApp
{
    public class BudgetService
    {
        private readonly IBudgetRepository _repo;

        public BudgetService(IBudgetRepository repo)
        {
            _repo = repo;
        }

        public decimal Query(DateTime startDate, DateTime endDate)
        {
            var budgets = this._repo.GetAll();
            string searchMonth = "";
            if (startDate > endDate)
            {
                return 0;
            }

            if (startDate.Year == endDate.Year && startDate.Month == endDate.Month)
            {
                searchMonth = startDate.ToString("yyyyMM");
                if (budgets.All(x => x.YearMonth != searchMonth))
                {
                    return 0;
                }
                      

                if (endDate.Day == DateTime.DaysInMonth(startDate.Year, startDate.Month) && startDate.Day == 1)
                { 
                    return budgets.FirstOrDefault(x => x.YearMonth == searchMonth).Amount;
                }
                else if (startDate.Day == endDate.Day)
                {
                    return budgets.FirstOrDefault(x => x.YearMonth == searchMonth).Amount / DateTime.DaysInMonth(startDate.Year, startDate.Month);

                }
                else
                {
                   
                        
                    return budgets.FirstOrDefault(x => x.YearMonth == searchMonth).Amount / DateTime.DaysInMonth(startDate.Year, startDate.Month) * (endDate.Day - startDate.Day + 1);
                }
            }
            else
            {
                var firstmonth =budgets.FirstOrDefault(x => x.YearMonth == startDate.ToString("yyyyMM")).Amount / DateTime.DaysInMonth(startDate.Year, startDate.Month) * (DateTime.DaysInMonth(startDate.Year, startDate.Month) - startDate.Day + 1);
                var secondmonth = budgets.FirstOrDefault(x => x.YearMonth == endDate.ToString("yyyyMM")).Amount / DateTime.DaysInMonth(endDate.Year, endDate.Month) * (endDate.Day);
                return firstmonth + secondmonth;
            }

           
        }

    }
}
