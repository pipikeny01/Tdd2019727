using System;
using System.Collections.Generic;
using System.Linq;
using BudgetApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace TestBudget
{
    [TestClass]
    public class BudgetServiceTests
    {
        private readonly IBudgetRepository _budgetRepository = Substitute.For<IBudgetRepository>();
        private readonly BudgetService _budgetService;

        public BudgetServiceTests()
        {
            _budgetService = new BudgetService(_budgetRepository);
        }

        [TestMethod]
        public void Test_BudgetZero()
        {
            GivenBudgets(new Budget {YearMonth = "201907", Amount = 0});
            decimal actual = _budgetService.Query(new DateTime(2019, 7, 27), new DateTime(2019, 7, 31));
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void Test_CrossThreeMonth()
        {
            GivenBudgets(
                new Budget {YearMonth = "201901", Amount = 31000},
                new Budget {YearMonth = "201902", Amount = 28000},
                new Budget {YearMonth = "201903", Amount = 31000}
            );

            decimal actual = _budgetService.Query(new DateTime(2019, 1, 31), new DateTime(2019, 3, 10));
            Assert.AreEqual(39000, actual);
        }

        [TestMethod]
        public void Test_CrossTwoMonth()
        {
            GivenBudgets(
                new Budget {YearMonth = "201901", Amount = 31000},
                new Budget {YearMonth = "201902", Amount = 28000}
            );
            decimal actual = _budgetService.Query(new DateTime(2019, 1, 31), new DateTime(2019, 2, 10));
            Assert.AreEqual(11000, actual);
        }

        [TestMethod]
        public void Test_InvalidRange()
        {
            GivenBudgets(
                new Budget {YearMonth = "201907", Amount = 31000}
            );
            decimal actual = _budgetService.Query(new DateTime(2019, 7, 27), new DateTime(2019, 6, 27));
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void Test_NoBudgetData()
        {
            _budgetRepository.GetAll().Returns(new List<Budget>());
            decimal actual = _budgetService.Query(new DateTime(2019, 7, 27), new DateTime(2019, 7, 31));
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void Test_PartialMonth()
        {
            GivenBudgets(new Budget {YearMonth = "201901", Amount = 31000});
            decimal actual = _budgetService.Query(new DateTime(2019, 1, 1), new DateTime(2019, 1, 10));
            Assert.AreEqual(10000, actual);
        }

        [TestMethod]
        public void Test_SingleDate()
        {
            GivenBudgets(
                new Budget {YearMonth = "201901", Amount = 31000}
            );
            decimal actual = _budgetService.Query(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1));
            Assert.AreEqual(1000, actual);
        }

        [TestMethod]
        public void Test_SingleWholeMonth()
        {
            GivenBudgets(
                new Budget {YearMonth = "201901", Amount = 31000}
            );
            decimal actual = _budgetService.Query(new DateTime(2019, 1, 1), new DateTime(2019, 1, 31));
            Assert.AreEqual(31000, actual);
        }

        private void GivenBudgets(params Budget[] budgets)
        {
            _budgetRepository.GetAll().Returns(budgets.ToList());
        }
    }
}