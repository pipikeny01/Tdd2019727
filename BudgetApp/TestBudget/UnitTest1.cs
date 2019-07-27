using System;
using System.Collections.Generic;
using BudgetApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace TestBudget
{
    [TestClass]
    public class UnitTest1
    {
        private BudgetService _budgetService;
        private IBudgetRepository _budgetRepository = Substitute.For<IBudgetRepository>();

        public UnitTest1()
        {
            _budgetService = new BudgetService(_budgetRepository);
        }
        [TestMethod]
        public void Test_InvaidRange()
        {

            decimal actual = _budgetService.Query(new DateTime(2019, 7, 27), new DateTime(2019, 6, 27));
            Assert.AreEqual( 0, actual);
        }

        [TestMethod]
        public void Test_NoBudgetData()
        {
            _budgetRepository.GetAll().Returns(new List<Budget>());
            decimal actual = _budgetService.Query(new DateTime(2019, 7, 27), new DateTime(2019, 7, 31));
            Assert.AreEqual( 0, actual);
        }
        [TestMethod]
        public void Test_BudgetZero()
        {
            List<Budget> mockBudgets=new List<Budget>();

            mockBudgets.Add(new Budget {YearMonth = "201907", Amount = 0});
            _budgetRepository.GetAll().Returns(mockBudgets);
            decimal actual = _budgetService.Query(new DateTime(2019, 7, 27), new DateTime(2019, 7, 31));
            Assert.AreEqual( 0, actual);
        }
        [TestMethod]
        public void Test_SingleWholeMonth()
        {
            List<Budget> mockBudgets=new List<Budget>();
            mockBudgets.Add(new Budget {YearMonth = "201901", Amount = 31000});
            _budgetRepository.GetAll().Returns(mockBudgets);
            decimal actual = _budgetService.Query(new DateTime(2019, 1, 1), new DateTime(2019, 1, 31));
            Assert.AreEqual( 31000, actual);
        }

        [TestMethod]
        public void Test_SingleDate()
        {
            List<Budget> mockBudgets=new List<Budget>();
            mockBudgets.Add(new Budget {YearMonth = "201901", Amount = 31000});
            _budgetRepository.GetAll().Returns(mockBudgets);
            decimal actual = _budgetService.Query(new DateTime(2019, 1, 1), new DateTime(2019, 1, 1));
            Assert.AreEqual( 1000, actual);
        }
        [TestMethod]
        public void Test_PartialMonth()
        {
            List<Budget> mockBudgets=new List<Budget>();
            mockBudgets.Add(new Budget {YearMonth = "201901", Amount = 31000});
            _budgetRepository.GetAll().Returns(mockBudgets);
            decimal actual = _budgetService.Query(new DateTime(2019, 1, 1), new DateTime(2019, 1, 10));
            Assert.AreEqual( 10000, actual);
        }

        [TestMethod]
        public void Test_CrossTwoMonth()
        {
            List<Budget> mockBudgets=new List<Budget>();
            mockBudgets.Add(new Budget {YearMonth = "201901", Amount = 31000});
            mockBudgets.Add(new Budget {YearMonth = "201902", Amount = 28000});
            _budgetRepository.GetAll().Returns(mockBudgets);
            decimal actual = _budgetService.Query(new DateTime(2019, 1, 31), new DateTime(2019, 2, 10));
            Assert.AreEqual( 11000, actual);
        }

        

    }
}
