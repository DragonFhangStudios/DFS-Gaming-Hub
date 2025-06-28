using DFS.JobSystem.Data;
using DFS.JobSystem.Managers;
using DFS.CharacterForge;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DFS.DevLauncher
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void LaunchPuzzle_Click(object sender, RoutedEventArgs e)
		{
			string puzzlePath = Path.GetFullPath(@"..\..\..\..\DFS.WPFLauncher\bin\Debug\net8.0-windows\DFS.WPFLauncher.exe");

			if (File.Exists(puzzlePath))
			{
				StatusLog.Text = "Launching Puzzle Framework...";
				Process.Start(puzzlePath);
			}
			else
			{
				StatusLog.Text = "❌ Puzzle Framework not found. Build it first!";
				MessageBox.Show("Puzzle Framework not found. Build it first!", "Launch Failed");
			}
		}

		private void LaunchSyndicate_Click(object sender, RoutedEventArgs e)
		{
			string syndicatePath = Path.GetFullPath(@"..\..\..\..\DFS.ProjectSyndicate\bin\Debug\net8.0-windows\DFS.ProjectSyndicate.exe");

			if (File.Exists(syndicatePath))
			{
				StatusLog.Text = "Launching Project Syndicate...";
				Process.Start(syndicatePath);
			}
			else
			{
				StatusLog.Text = "❌ Project Syndicate not found. Build it first!";
				MessageBox.Show("Project Syndicate not found. Build it first!", "Launch Failed");
			}
		}
		private void LaunchCharacterForge_Click(object sender, RoutedEventArgs e)
		{
			string forgePath = Path.GetFullPath(@"..\..\..\..\DFS.CharacterForge\bin\Debug\net8.0-windows\DFS.CharacterForge.exe");

			if (File.Exists(forgePath))
			{
				StatusLog.Text = "Launching Character Forge...";
				Process.Start(forgePath);
			}
			else
			{
				StatusLog.Text = "❌ Character Forge not found. Build it first!";
				MessageBox.Show("Character Forge not found. Build it first!", "Launch Failed");
			}
		}
		/*private void RunDeliveryJob_Click(object sender, RoutedEventArgs e)
		{
			JobOutputPanel.Items.Clear();

			var jobManager = new JobManager();
			JobLoader.RegisterAllJobs(jobManager);

			var job = jobManager.GetJob("delivery_driver");

			if (job == null)
			{
				JobOutputPanel.Items.Add("❌ Job not found.");
				return;
			}

			JobOutputPanel.Items.Add($"💼 Job: {job.Title} - Tier {job.Tier}");
			JobOutputPanel.Items.Add($"📄 Description: {job.Description}");
			JobOutputPanel.Items.Add("-----");

			int totalPay = 0;

			foreach (var task in job.Tasks)
			{
				task.IsCompleted = true; // Simulate completion
				totalPay += task.Reward;
				JobOutputPanel.Items.Add($"✔️ {task.Name} - ${task.Reward}");
			}

			JobOutputPanel.Items.Add("-----");
			JobOutputPanel.Items.Add($"💰 Total Payout: ${totalPay}");
		}*/
	}
}
