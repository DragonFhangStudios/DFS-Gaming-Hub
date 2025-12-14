using CNCMachinistSim.Models;
using CNCMachinistSim.Services;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace CNCMachinistSim
{
	public partial class MainWindow : Window
	{
		private GameManager _game;

		public MainWindow()
		{
			InitializeComponent();
			_game = GameManager.Instance;
		}

		private void NewGame_Click(object sender, RoutedEventArgs e)
		{
			_game.NewGame();
			UpdateUI();
		}

		private async void AcceptJob_Click(object sender, RoutedEventArgs e)
		{
			// PREVENT multiple jobs
			if (_game.IsJobRunning)
			{
				MessageBox.Show("Job already in progress!", "Wait");
				return;
			}

			if (sender is Button btn && btn.Tag is WorkOrder job)
			{
				// Open job setup dialog
				var setupDialog = new Views.JobSetupWindow(job, _game.CurrentPlayer.OwnedTools);

				if (setupDialog.ShowDialog() == true)
				{
					string strategy = setupDialog.SelectedStrategy;

					_game.AcceptJob(job);
					UpdateUI();

					// Show job running state
					NoJobPanel.Visibility = Visibility.Collapsed;
					JobRunningPanel.Visibility = Visibility.Visible;
					ActiveJobTitle.Text = job.PartType;
					ActiveJobStrategy.Text = $"Strategy: {strategy}";

					// Start the job (this is async - runs in background)
					await RunJobWithProgressAsync(strategy);
				}
			}
		}

		private async Task RunJobWithProgressAsync(string strategy)
		{
			// Start the job
			var jobTask = _game.StartJobAsync(strategy);

			// Update progress bar while job runs
			while (_game.IsJobRunning)
			{
				JobProgressBar.Value = _game.JobProgress;
				JobProgressText.Text = $"{_game.JobProgress}%";
				await Task.Delay(100); // Update UI every 100ms
			}

			// Job complete
			JobProgressBar.Value = 100;
			JobProgressText.Text = "100%";

			await Task.Delay(500); // Brief pause to show completion

			// Show result
			bool success = _game.CurrentPlayer.JobsCompleted > 0; // Check if job was successful
			string result = success ? "✓ PASS - Part within tolerance!" : "✗ FAIL - Part out of spec (scrap)";
			MessageBox.Show(result, "Quality Check");

			// Reset UI
			JobRunningPanel.Visibility = Visibility.Collapsed;
			NoJobPanel.Visibility = Visibility.Visible;
			UpdateUI();
		}

		private void UpdateUI()
		{
			// Update money and day
			MoneyText.Text = _game.CurrentPlayer.Money.ToString("F2");
			DayText.Text = _game.CurrentPlayer.CurrentDay.ToString();

			// Update work orders
			JobListView.ItemsSource = null;
			JobListView.ItemsSource = _game.AvailableJobs;

			// Update tools
			ToolListView.ItemsSource = null;
			ToolListView.ItemsSource = _game.CurrentPlayer.OwnedTools;
			JobListView.IsEnabled = !_game.IsJobRunning;
		}
		private void OpenToolShop_Click(object sender, RoutedEventArgs e)
		{
			var toolShop = new Views.ToolShopWindow();
			toolShop.ShowDialog();
			UpdateUI(); // Refresh money display after shop closes
		}
	}
}