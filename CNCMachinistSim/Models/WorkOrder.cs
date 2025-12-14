using System.Collections.Generic;

namespace CNCMachinistSim.Models
{
	public class WorkOrder
	{
		public string PartType { get; set; }
		public decimal BasePay { get; set; }
		public decimal MaterialCost { get; set; }
		public double ToleranceInches { get; set; }
		public List<string> RequiredTools { get; set; }
		public int JobNumber { get; set; }

		// Unlock system properties
		public int UnlockAtJobs { get; set; }
		public bool IsUnlocked { get; set; }
		public bool IsLocked => !IsUnlocked;
		public string UnlockMessage => IsLocked ? $"🔒 Unlock at {UnlockAtJobs} jobs" : "";

		public WorkOrder(string partType, decimal basePay, decimal materialCost,
						 double tolerance, List<string> requiredTools, int jobNum, int unlockAt = 0)
		{
			PartType = partType;
			BasePay = basePay;
			MaterialCost = materialCost;
			ToleranceInches = tolerance;
			RequiredTools = requiredTools;
			JobNumber = jobNum;
			UnlockAtJobs = unlockAt;
			IsUnlocked = (unlockAt == 0);
		}

		// Factory methods
		public static WorkOrder CreatePin(int jobNum)
		{
			return new WorkOrder(
				"Aluminum Pin",
				25m,
				5m,
				0.005,
				new List<string> { "FaceMill", "Drill" },
				jobNum,
				0
			);
		}

		public static WorkOrder CreateBushing(int jobNum, bool isUnlocked = false)
		{
			var order = new WorkOrder(
				"Aluminum Bushing",
				40m,
				8m,
				0.003,
				new List<string> { "FaceMill", "Drill" },
				jobNum,
				5
			);
			order.IsUnlocked = isUnlocked;
			return order;
		}

		public static WorkOrder CreateSpacer(int jobNum, bool isUnlocked = false)
		{
			var order = new WorkOrder(
				"Aluminum Spacer",
				35m,
				7m,
				0.002,
				new List<string> { "FaceMill", "Chamfer" },
				jobNum,
				10
			);
			order.IsUnlocked = isUnlocked;
			return order;
		}

		public static WorkOrder CreateFitting(int jobNum, bool isUnlocked = false)
		{
			var order = new WorkOrder(
				"Aluminum Fitting",
				50m,
				10m,
				0.005,
				new List<string> { "Drill", "Chamfer" },
				jobNum,
				15
			);
			order.IsUnlocked = isUnlocked;
			return order;
		}

		public static WorkOrder CreateBracket(int jobNum, bool isUnlocked = false)
		{
			var order = new WorkOrder(
				"Aluminum Bracket",
				75m,
				15m,
				0.002,
				new List<string> { "FaceMill", "Drill", "Chamfer" },
				jobNum,
				20
			);
			order.IsUnlocked = isUnlocked;
			return order;
		}
	}
}