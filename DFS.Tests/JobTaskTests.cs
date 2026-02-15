using DFS.JobSystem.Core;
using Xunit;
using System;

namespace DFS.Tests
{
    public class JobTaskTests
    {
        [Fact]
        public void Constructor_WithNegativeReward_ShouldThrowException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new JobTask("Test", "Test Desc", -10));
        }

        [Fact]
        public void Reward_SetNegativeValue_ShouldThrowException()
        {
            var task = new JobTask("Test", "Test Desc", 10);
            Assert.Throws<ArgumentOutOfRangeException>(() => task.Reward = -5);
        }

        [Fact]
        public void Reward_SetPositiveValue_ShouldUpdateValue()
        {
            var task = new JobTask("Test", "Test Desc", 10);
            task.Reward = 20;
            Assert.Equal(20, task.Reward);
        }
    }
}
