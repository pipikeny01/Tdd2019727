using System;

namespace BudgetApp
{
    public class Budget
    {
        public string YearMonth { get; set; }
        public int Amount { get; set; }
        public DateTime LastDay => DateTime.ParseExact(YearMonth + Days(), "yyyyMMdd", null);

        public DateTime FirstDay => DateTime.ParseExact(YearMonth + "01", "yyyyMMdd", null);

        public int Days()
        {
            var fistDay = DateTime.ParseExact(YearMonth+"01","yyyyMMdd",null);
            return DateTime.DaysInMonth(fistDay.Year, fistDay.Month);
        }

        public int DailyAmount()
        {
            return Amount / Days();
        }
    }
}