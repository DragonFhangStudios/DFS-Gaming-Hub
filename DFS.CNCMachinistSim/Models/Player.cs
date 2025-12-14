using System.Collections.Generic;

namespace CNCMachinistSim.Models
{
	public class Player
	{
		public decimal Money { get; set; }
		public List<Tool> OwnedTools { get; set; }
		public int JobsCompleted { get; set; }
		public int JobsFailed { get; set; }
		public int CurrentDay { get; set; }

		public Player()
		{
			Money = 500m; // Starting capital
			OwnedTools = new List<Tool>();
			JobsCompleted = 0;
			JobsFailed = 0;
			CurrentDay = 1;

			// Starting tools (all worn)
			OwnedTools.Add(new Tool("HSS End Mill #1", "EndMill", 15m, 50));
			OwnedTools.Add(new Tool("HSS End Mill #2", "EndMill", 15m, 50));
			OwnedTools.Add(new Tool("Drill Bit 1/4\"", "Drill", 10m, 60));
			OwnedTools.Add(new Tool("Drill Bit 3/8\"", "Drill", 10m, 60));
			OwnedTools.Add(new Tool("Face Mill", "FaceMill", 25m, 40));
			OwnedTools.Add(new Tool("Chamfer Tool", "Chamfer", 12m, 80));
		}

		public bool CanAfford(decimal cost) => Money >= cost;

		public void Pay(decimal amount)
		{
			Money += amount;
		}

		public void Charge(decimal amount)
		{
			Money -= amount;
		}
	}
}