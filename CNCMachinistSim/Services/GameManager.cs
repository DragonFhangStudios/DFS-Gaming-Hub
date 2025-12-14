using System;
using System.Collections.Generic;
using CNCMachinistSim.Models;

namespace CNCMachinistSim.Services
{
	public class GameManager
	{
		private static GameManager _instance;
		public static GameManager Instance => _instance ??= new GameManager();

		public Player CurrentPlayer { get; private set; }
		public List<WorkOrder> AvailableJobs { get; private set; }
		public WorkOrder ActiveJob { get; private set; }

		private int _nextJobNumber = 1;
		public bool IsJobRunning { get; private set; }
		public int JobProgress { get; private set; }  // 0-100%
		public JobStrategy CurrentStrategy { get; private set; }
		public bool CanUnlockBushings => CurrentPlayer.JobsCompleted >= 5;
		public bool CanUnlockSpacers => CurrentPlayer.JobsCompleted >= 10;
		public bool CanUnlockFittings => CurrentPlayer.JobsCompleted >= 15;
		public bool CanUnlockBrackets => CurrentPlayer.JobsCompleted >= 20;

		private GameManager()
		{
			AvailableJobs = new List<WorkOrder>();
		}

		public void NewGame()
		{
			CurrentPlayer = new Player();
			ActiveJob = null;
			_nextJobNumber = 1;
			GenerateWorkOrders();
		}

		private void GenerateWorkOrders()
		{
			AvailableJobs.Clear();

			int completed = CurrentPlayer.JobsCompleted;

			// Generate 5 total jobs (mix of available + locked)

			// Always offer at least one Pin (unlocked)
			AvailableJobs.Add(WorkOrder.CreatePin(_nextJobNumber++));

			// Show Bushing (unlocked if 5+ jobs completed)
			AvailableJobs.Add(WorkOrder.CreateBushing(_nextJobNumber++, completed >= 5));

			// Show Spacer (unlocked if 10+ jobs completed)
			AvailableJobs.Add(WorkOrder.CreateSpacer(_nextJobNumber++, completed >= 10));

			// Show Fitting (unlocked if 15+ jobs completed)  
			AvailableJobs.Add(WorkOrder.CreateFitting(_nextJobNumber++, completed >= 15));

			// Show Bracket (unlocked if 20+ jobs completed)
			AvailableJobs.Add(WorkOrder.CreateBracket(_nextJobNumber++, completed >= 20));
		}

		public void AcceptJob(WorkOrder job)
		{
			if (!AvailableJobs.Contains(job)) return;

			ActiveJob = job;
			AvailableJobs.Remove(job);

			// Deduct material cost immediately
			CurrentPlayer.Charge(job.MaterialCost);

			// Generate new job to replace it
			AvailableJobs.Add(WorkOrder.CreatePin(_nextJobNumber++));
		}

		public void CompleteJob(bool success)
		{
			if (ActiveJob == null) return;

			if (success)
			{
				// Full payment
				CurrentPlayer.Pay(ActiveJob.BasePay);
				CurrentPlayer.JobsCompleted++;
			}
			else
			{
				// Scrap value (50% of payment)
				CurrentPlayer.Pay(ActiveJob.BasePay * 0.5m);
				CurrentPlayer.JobsFailed++;
			}

			ActiveJob = null;
		}
		public async Task StartJobAsync(string strategyName)
		{
			if (ActiveJob == null) return;

			CurrentStrategy = JobStrategy.FromString(strategyName);
			IsJobRunning = true;
			JobProgress = 0;

			// Base job duration: 10 seconds (for testing - will adjust later)
			int baseDuration = 10;
			int actualDuration = (int)(baseDuration * CurrentStrategy.TimeMultiplier);

			// Simulate job progress
			int steps = 20; // Update progress 20 times
			int delayPerStep = (actualDuration * 1000) / steps;

			for (int i = 0; i <= steps; i++)
			{
				JobProgress = (i * 100) / steps;
				await Task.Delay(delayPerStep);
			}

			// Job complete - now do quality check
			IsJobRunning = false;
			PerformQualityCheck();
		}

		private void PerformQualityCheck()
		{
			// Get all tools used for this job
			var usedTools = CurrentPlayer.OwnedTools
				.Where(t => ActiveJob.RequiredTools.Contains(t.Type))
				.ToList();

			// Calculate success chance
			int baseSuccess = CurrentStrategy.SuccessRate;

			// Reduce success rate if tools are worn
			foreach (var tool in usedTools)
			{
				if (tool.Condition < 80)
				{
					baseSuccess -= (80 - tool.Condition) / 10; // Lose ~1% per 10% tool wear
				}

				// Broken tools = auto-fail
				if (tool.IsBroken)
				{
					baseSuccess = 0;
					break;
				}
			}

			// Roll the dice
			Random rng = new Random();
			bool success = rng.Next(0, 100) < baseSuccess;

			// Apply tool wear
			foreach (var tool in usedTools)
			{
				tool.ApplyWear(CurrentStrategy.ToolWearPercent / 10); // Convert percent to multiplier
			}

			// Complete the job
			decimal finalPay = ActiveJob.BasePay * (decimal)CurrentStrategy.PayMultiplier;
			CompleteJob(success, finalPay);
		}

		public void CompleteJob(bool success, decimal finalPay)
		{
			if (ActiveJob == null) return;

			if (success)
			{
				CurrentPlayer.Pay(finalPay);
				CurrentPlayer.JobsCompleted++;
			}
			else
			{
				// Scrap value (50% of base pay, no bonus)
				CurrentPlayer.Pay(ActiveJob.BasePay * 0.5m);
				CurrentPlayer.JobsFailed++;
			}

			ActiveJob = null;
			CheckRecurringExpenses();
		}
		public void CheckRecurringExpenses()
		{
			// Rent every 5 jobs
			if (CurrentPlayer.JobsCompleted % 5 == 0 && CurrentPlayer.JobsCompleted > 0)
			{
				CurrentPlayer.Charge(25m);
				// Show notification in UI
			}

			// Machine maintenance every 10 jobs
			if (CurrentPlayer.JobsCompleted % 10 == 0 && CurrentPlayer.JobsCompleted > 0)
			{
				CurrentPlayer.Charge(50m);
				// Show notification in UI
			}
		}
	}
	public class JobStrategy
	{
		public string Name { get; set; }
		public double TimeMultiplier { get; set; }  // 0.5 = half time, 1.0 = normal
		public int SuccessRate { get; set; }        // 0-100%
		public double PayMultiplier { get; set; }   // 1.25 = 125% pay
		public int ToolWearPercent { get; set; }    // How much tools degrade

		public static JobStrategy Conservative => new JobStrategy
		{
			Name = "Conservative",
			TimeMultiplier = 1.5,  // Takes 50% longer
			SuccessRate = 95,
			PayMultiplier = 1.0,
			ToolWearPercent = 5
		};

		public static JobStrategy Normal => new JobStrategy
		{
			Name = "Normal",
			TimeMultiplier = 1.0,
			SuccessRate = 85,
			PayMultiplier = 1.1,
			ToolWearPercent = 10
		};

		public static JobStrategy Aggressive => new JobStrategy
		{
			Name = "Aggressive",
			TimeMultiplier = 0.5,  // Half the time!
			SuccessRate = 60,
			PayMultiplier = 1.25,
			ToolWearPercent = 20
		};

		public static JobStrategy FromString(string name)
		{
			return name switch
			{
				"Conservative" => Conservative,
				"Normal" => Normal,
				"Aggressive" => Aggressive,
				_ => Normal
			};
		}
	}
}