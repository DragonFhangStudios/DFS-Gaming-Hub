using System.Collections.Generic;

namespace CNCMachinistSim.Models
{
	public class WorkOrder
	{
		public string PartType { get; set; } // "Pin", "Bushing", "Spacer", etc.
		public decimal BasePay { get; set; }
		public decimal MaterialCost { get; set; }
		public double ToleranceInches { get; set; } // ±0.005", etc.
		public List<string> RequiredTools { get; set; }
		public int JobNumber { get; set; }

		public WorkOrder(string partType, decimal basePay, decimal materialCost,
						 double tolerance, List<string> requiredTools, int jobNum)
		{
			PartType = partType;
			BasePay = basePay;
			MaterialCost = materialCost;
			ToleranceInches = tolerance;
			RequiredTools = requiredTools;
			JobNumber = jobNum;
		}

		// Factory methods for each part type
		public static WorkOrder CreatePin(int jobNum)
		{
			return new WorkOrder(
				"Aluminum Pin",
				25m,
				5m,
				0.005,
				new List<string> { "FaceMill", "Drill" },
				jobNum
			);
		}

		public static WorkOrder CreateBushing(int jobNum)
		{
			return new WorkOrder(
				"Aluminum Bushing",
				40m,
				8m,
				0.003,
				new List<string> { "FaceMill", "Drill", "Bore" },
				jobNum
			);
		}

		// Add more part types later
	}
}