using System;
using System.Collections.Generic;
using Xunit;
using CNCMachinistSim.Services;
using CNCMachinistSim.Models;

namespace DFS.Tests
{
    public class GameManagerTests
    {
        public GameManagerTests()
        {
            // Reset state before each test
            GameManager.Instance.NewGame();
        }

        [Fact]
        public void NewGame_ResetsState()
        {
            var gm = GameManager.Instance;
            Assert.Equal(0, gm.CurrentPlayer.JobsCompleted);
            Assert.Equal(500m, gm.CurrentPlayer.Money);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(9)]
        public void CheckRecurringExpenses_NoExpensesTriggered_WhenNotMultipleOfInterval(int jobsCompleted)
        {
            // Arrange
            var gm = GameManager.Instance;
            gm.CurrentPlayer.JobsCompleted = jobsCompleted;
            decimal initialMoney = gm.CurrentPlayer.Money;

            // Act
            var expenses = gm.CheckRecurringExpenses();

            // Assert
            Assert.Empty(expenses);
            Assert.Equal(initialMoney, gm.CurrentPlayer.Money);
        }

        [Fact]
        public void CheckRecurringExpenses_RentTriggered_AtFiveJobs()
        {
            // Arrange
            var gm = GameManager.Instance;
            gm.CurrentPlayer.JobsCompleted = 5;
            decimal initialMoney = gm.CurrentPlayer.Money;
            decimal rentAmount = 25m;

            // Act
            var expenses = gm.CheckRecurringExpenses();

            // Assert
            Assert.Single(expenses);
            Assert.Contains("Shop rent", expenses[0]);
            Assert.Equal(initialMoney - rentAmount, gm.CurrentPlayer.Money);
        }

        [Fact]
        public void CheckRecurringExpenses_BothRentAndMaintenanceTriggered_AtTenJobs()
        {
            // Arrange
            var gm = GameManager.Instance;
            gm.CurrentPlayer.JobsCompleted = 10;
            decimal initialMoney = gm.CurrentPlayer.Money;
            decimal rentAmount = 25m;
            decimal maintenanceAmount = 50m;

            // Act
            var expenses = gm.CheckRecurringExpenses();

            // Assert
            Assert.Equal(2, expenses.Count);
            Assert.Contains(expenses, e => e.Contains("Shop rent"));
            Assert.Contains(expenses, e => e.Contains("Machine maintenance"));
            Assert.Equal(initialMoney - rentAmount - maintenanceAmount, gm.CurrentPlayer.Money);
        }

        [Fact]
        public void CheckRecurringExpenses_ExpensesNotTriggeredTwice_ForSameJobCount()
        {
            // Arrange
            var gm = GameManager.Instance;
            gm.CurrentPlayer.JobsCompleted = 5;
            decimal initialMoney = gm.CurrentPlayer.Money;
            decimal rentAmount = 25m;

            // Act 1: First check triggers rent
            var expenses1 = gm.CheckRecurringExpenses();
            Assert.Single(expenses1);
            Assert.Equal(initialMoney - rentAmount, gm.CurrentPlayer.Money);

            // Act 2: Second check should do nothing
            var expenses2 = gm.CheckRecurringExpenses();

            // Assert
            Assert.Empty(expenses2);
            Assert.Equal(initialMoney - rentAmount, gm.CurrentPlayer.Money); // Money should not change further
        }

        [Fact]
        public void CheckRecurringExpenses_MaintenanceNotTriggeredTwice_ForSameJobCount()
        {
             // Arrange
            var gm = GameManager.Instance;
            gm.CurrentPlayer.JobsCompleted = 10;
            decimal initialMoney = gm.CurrentPlayer.Money;
            decimal rentAmount = 25m;
            decimal maintenanceAmount = 50m;

            // Act 1: First check triggers rent and maintenance
            var expenses1 = gm.CheckRecurringExpenses();
            Assert.Equal(2, expenses1.Count);
            Assert.Equal(initialMoney - rentAmount - maintenanceAmount, gm.CurrentPlayer.Money);

            // Act 2: Second check should do nothing
            var expenses2 = gm.CheckRecurringExpenses();

            // Assert
            Assert.Empty(expenses2);
            Assert.Equal(initialMoney - rentAmount - maintenanceAmount, gm.CurrentPlayer.Money); // Money should not change further
        }
    }
}
