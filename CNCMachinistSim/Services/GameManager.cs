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
		public WorkOrder FailedJob { get; private set; } // Track the job that failed
		public int CurrentRetryCount { get; private set; } // How many times we've retried THIS job

		private const int RentJobInterval = 5;
		private const decimal RentAmount = 25m;
		private const int MaintenanceJobInterval = 10;
		private const decimal MaintenanceAmount = 50m;

		private const int BushingUnlockThreshold = 5;
		private const int SpacerUnlockThreshold = 10;
		private const int FittingUnlockThreshold = 15;
		private const int BracketUnlockThreshold = 20;

		private int _nextJobNumber = 1;
		public bool IsJobRunning { get; private set; }
		public int JobProgress { get; private set; }  // 0-100%
		public JobStrategy CurrentStrategy { get; private set; }
		public bool CanUnlockBushings => CurrentPlayer.JobsCompleted >= BushingUnlockThreshold;
		public bool CanUnlockSpacers => CurrentPlayer.JobsCompleted >= SpacerUnlockThreshold;
		public bool CanUnlockFittings => CurrentPlayer.JobsCompleted >= FittingUnlockThreshold;
		public bool CanUnlockBrackets => CurrentPlayer.JobsCompleted >= BracketUnlockThreshold;
		public bool LastJobSuccess { get; private set; }
		private int _lastRentJob = 0;
		private int _lastMaintenanceJob = 0;

		private GameManager()
		{
			AvailableJobs = new List<WorkOrder>();
		}

		public void NewGame()
		{
			CurrentPlayer = new Player();
			ActiveJob = null;
			_nextJobNumber = 1;
			_lastRentJob = 0;           // RESET
			_lastMaintenanceJob = 0;    // RESET
			GenerateWorkOrders();
		}

		public void GenerateWorkOrders()
		{
			AvailableJobs.Clear();

			int completed = CurrentPlayer.JobsCompleted;

			// Generate 5 total jobs (mix of available + locked)

			// Always offer at least one Pin (unlocked)
			AvailableJobs.Add(WorkOrder.CreatePin(_nextJobNumber++));

			// Show Bushing (unlocked if threshold reached)
			AvailableJobs.Add(WorkOrder.CreateBushing(_nextJobNumber++, completed >= BushingUnlockThreshold));

			// Show Spacer (unlocked if threshold reached)
			AvailableJobs.Add(WorkOrder.CreateSpacer(_nextJobNumber++, completed >= SpacerUnlockThreshold));

			// Show Fitting (unlocked if threshold reached)
			AvailableJobs.Add(WorkOrder.CreateFitting(_nextJobNumber++, completed >= FittingUnlockThreshold));

			// Show Bracket (unlocked if threshold reached)
			AvailableJobs.Add(WorkOrder.CreateBracket(_nextJobNumber++, completed >= BracketUnlockThreshold));
		}

		public void AcceptJob(WorkOrder job)
		{
			if (!AvailableJobs.Contains(job)) return;

			ActiveJob = job;
			AvailableJobs.Remove(job);

			// Deduct material cost immediately
			CurrentPlayer.Charge(job.MaterialCost);

			// Generate new job to replace it
			GenerateWorkOrders(); // This refreshes the list with current unlock status
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
			// Apply tool wear (DOUBLE for retry)
			int wearMultiplier = CurrentStrategy.Name == "ConservativeRetry" ? 2 : 1;
			foreach (var tool in usedTools)
			{
				tool.ApplyWear((CurrentStrategy.ToolWearPercent / 10) * wearMultiplier);
			}

			// Complete the job
			decimal finalPay = ActiveJob.BasePay * (decimal)CurrentStrategy.PayMultiplier;
			CompleteJob(success, finalPay);
		}

		public void CompleteJob(bool success, decimal finalPay)
		{
			if (ActiveJob == null) return;

			LastJobSuccess = success; // TRACK THIS

			if (success)
			{
				// Full payment
				CurrentPlayer.Pay(finalPay);
				CurrentPlayer.JobsCompleted++;

				// Clear failed job state
				FailedJob = null;
				CurrentRetryCount = 0;
			}
			else
			{
				// Failed - scrap value
				decimal scrapValue = ActiveJob.MaterialCost * 0.5m;
				CurrentPlayer.Pay(scrapValue);
				CurrentPlayer.JobsFailed++;

				// Store failed job for retry option
				FailedJob = ActiveJob;
				CurrentRetryCount++;
			}
			ActiveJob = null;
		}
		public void RetryJob()
		{
			if (FailedJob == null) return;

			// Set the failed job as active again
			ActiveJob = FailedJob;

			// Charge DOUBLE material cost (buying new stock + wasted material)
			CurrentPlayer.Charge(FailedJob.MaterialCost * 2);

			// Don't clear FailedJob yet - wait until success or another failure
		}

		public void DeclineRetry()
		{
			// Player chose to take the loss
			FailedJob = null;
			CurrentRetryCount = 0;

			// Generate new jobs to replace it
			GenerateWorkOrders();
		}

		public List<string> CheckRecurringExpenses()
		{
			var expenses = new List<string>();
			int completed = CurrentPlayer.JobsCompleted;

			// Rent every X jobs
			if (completed > 0 && completed % RentJobInterval == 0 && _lastRentJob != completed)
			{
				CurrentPlayer.Charge(RentAmount);
				expenses.Add($"📋 Shop rent: -{RentAmount:C2}");
				_lastRentJob = completed;
			}

			// Machine maintenance every Y jobs
			if (completed > 0 && completed % MaintenanceJobInterval == 0 && _lastMaintenanceJob != completed)
			{
				CurrentPlayer.Charge(MaintenanceAmount);
				expenses.Add($"🔧 Machine maintenance: -{MaintenanceAmount:C2}");
				_lastMaintenanceJob = completed;
			}

			return expenses;
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
		public static JobStrategy ConservativeRetry => new JobStrategy
		{
			Name = "ConservativeRetry",
			TimeMultiplier = 1.5,
			SuccessRate = 95,
			PayMultiplier = 1.0,
			ToolWearPercent = 10  // DOUBLE the normal 5% conservative wear
		};
	}
}