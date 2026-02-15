using System;

namespace DFS.JobSystem.Core
{
	public class JobTask
	{
		private int _reward;

		public string Name { get; set; }
		public string Description { get; set; }

		public int Reward
		{
			get => _reward;
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException(nameof(Reward), "Reward cannot be negative.");
				_reward = value;
			}
		} // payout in $ or points

		public bool IsCompleted { get; set; } = false;

		public JobTask(string name, string description, int reward)
		{
			Name = name;
			Description = description;
			Reward = reward;
		}
	}
}
