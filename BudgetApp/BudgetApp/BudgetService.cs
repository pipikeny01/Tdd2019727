using System;
using System.Collections.Generic;
using System.Linq;

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
            if (startDate > endDate)
            {
                return 0;
            }

            //同年月
            if (startDate.ToString("yyyyMM") == endDate.ToString("yyyyMM"))
            {
                return QeurySingleMonth(startDate, endDate, budgets);
            }
            else
            {
                var startBudget = budgets.FirstOrDefault(x => x.YearMonth == startDate.ToString("yyyyMM"));
                int firstMonth = 0;
                if (startBudget != null)
                {
                    firstMonth = startBudget.Amount /
                                 DateTime.DaysInMonth(startDate.Year, startDate.Month) *
                                 (DateTime.DaysInMonth(startDate.Year, startDate.Month) - startDate.Day + 1);
                }

                var endBudget = budgets.FirstOrDefault(x => x.YearMonth == endDate.ToString("yyyyMM"));

                int endMonth = 0;
                if (endBudget != null)
                {
                    endMonth = endBudget.Amount /
                                  DateTime.DaysInMonth(endDate.Year, endDate.Month) * (endDate.Day);
                }

                var totalAmount = firstMonth + endMonth;

                //中間
                var allStartMonth = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1);
                var allEndMonth = new DateTime(endDate.Year, endDate.Month, 1);
                while (allEndMonth > allStartMonth)
                {
                    string searchMonth = "";

                    searchMonth = allStartMonth.ToString("yyyyMM");
                    if (budgets.Any(x => x.YearMonth == searchMonth))
                        totalAmount += budgets.FirstOrDefault(x => x.YearMonth == searchMonth).Amount;

                    allStartMonth = allStartMonth.AddMonths(1);
                }

                return totalAmount;
            }
        }

        private static decimal QeurySingleMonth(DateTime startDate, DateTime endDate, List<Budget> budgets)
        {
            string searchMonth = startDate.ToString("yyyyMM");

            if (budgets.All(x => x.YearMonth != searchMonth))
            {
                return 0;
            }

            //一整個月
            var budget = budgets.FirstOrDefault(x => x.YearMonth == searchMonth);
            if (endDate.Day == DateTime.DaysInMonth(startDate.Year, startDate.Month) && startDate.Day == 1)
            {
                return budget.Amount;
            }
            else if (startDate.Day == endDate.Day) //同一天
            {
                return budget.Amount /
                       DateTime.DaysInMonth(startDate.Year, startDate.Month);
            }
            else
            {
                return budget.Amount /
                       DateTime.DaysInMonth(startDate.Year, startDate.Month) * (endDate.Day - startDate.Day + 1);
            }
        }
    }
}