namespace CNCMachinistSim.Models
{
	public class Tool
	{
		public string Name { get; set; }
		public string Type { get; set; } // "EndMill", "Drill", "FaceMill", "Chamfer"
		public int Condition { get; set; } // 0-100%
		public decimal ReplacementCost { get; set; }
		public int WearPerJob { get; set; } // How much condition degrades per use

		public Tool(string name, string type, decimal cost, int startingCondition = 100)
		{
			Name = name;
			Type = type;
			ReplacementCost = cost;
			Condition = startingCondition;
			WearPerJob = 10; // Default 10% wear per job
		}

		public void ApplyWear(int multiplier = 1)
		{
			Condition -= WearPerJob * multiplier;
			if (Condition < 0) Condition = 0;
		}

		public bool IsBroken => Condition <= 0;
		public bool NeedsReplacement => Condition < 40;
	}
}