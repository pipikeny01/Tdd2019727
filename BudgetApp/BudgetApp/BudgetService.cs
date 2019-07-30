using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetApp
{
    //https://github.com/201907tdd/tdd20190727
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
            if (IsSameMonth(startDate, endDate))
            {
                var budget = budgets.FirstOrDefault(x => x.YearMonth == startDate.ToString("yyyyMM"));

                if (budget == null) return 0;

                return budget.DailyAmount() * EffectiveDayCount(startDate, endDate);
            }
            else
            {
                var startBudget = budgets.FirstOrDefault(x => x.YearMonth == startDate.ToString("yyyyMM"));
                var totalAmount = 0m;
                int firstMonthAmount = 0;
                if (startBudget != null)
                {
                    firstMonthAmount = startBudget.DailyAmount() *
                                 EffectiveDayCount(startDate, startBudget.LastDay);
                    //(startBudget.Days() - startDate.Day + 1);
                }

                totalAmount += firstMonthAmount;
                var endBudget = budgets.FirstOrDefault(x => x.YearMonth == endDate.ToString("yyyyMM"));

                int endMonthAmount = 0;
                if (endBudget != null)
                {
                    endMonthAmount = endBudget.DailyAmount() * EffectiveDayCount(endBudget.FirstDay, endDate);
                }

                totalAmount += endMonthAmount;

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

        private static int EffectiveDayCount(DateTime startDate, DateTime budgetLastDay)
        {
            return ((budgetLastDay - startDate).Days + 1);
        }

        private static bool IsSameMonth(DateTime startDate, DateTime endDate)
        {
            return startDate.ToString("yyyyMM") == endDate.ToString("yyyyMM");
        }

        private static decimal QuerySingleMonth(DateTime startDate, DateTime endDate, List<Budget> budgets)
        {
            var budget = budgets.FirstOrDefault(x => x.YearMonth == startDate.ToString("yyyyMM"));

            if (budget == null) return 0;

            return budget.DailyAmount() * (endDate.Day - startDate.Day + 1);
        }
    }
}