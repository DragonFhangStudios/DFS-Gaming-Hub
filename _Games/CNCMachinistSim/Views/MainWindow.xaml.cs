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
				await Task.Delay(100);
			}

			// Job complete
			JobProgressBar.Value = 100;
			JobProgressText.Text = "100%";

			await Task.Delay(500);

			// Show quality check result
			bool success = _game.LastJobSuccess;
			string result = success ? "✓ PASS - Part within tolerance!" : "✗ FAIL - Part out of spec (scrap)";
			MessageBox.Show(result, "Quality Check");

			// NEW: If failed, offer retry
			if (!success && _game.FailedJob != null)
			{
				var retryDialog = new Views.RetryJobDialog(_game.FailedJob, _game.CurrentRetryCount);

				if (retryDialog.ShowDialog() == true)
				{
					if (retryDialog.ShouldRetry)
					{
						// Player chose to retry
						_game.RetryJob();
						UpdateUI();

						// Run the job again with CONSERVATIVE strategy and DOUBLE tool wear
						await RunRetryJobAsync();
						return; // Don't show expenses yet, wait for retry to finish
					}
					else
					{
						// Player declined retry
						_game.DeclineRetry();
					}
				}
			}

			// Check for recurring expenses
			var expenses = _game.CheckRecurringExpenses();
			if (expenses.Count > 0)
			{
				string expenseMessage = "The following expenses have been deducted:\n\n" +
									   string.Join("\n", expenses) +
									   $"\n\nNew balance: ${_game.CurrentPlayer.Money:F2}";
				MessageBox.Show(expenseMessage, "Recurring Expenses", MessageBoxButton.OK, MessageBoxImage.Information);
			}

			// Refresh jobs
			_game.GenerateWorkOrders();

			// Reset UI
			JobRunningPanel.Visibility = Visibility.Collapsed;
			NoJobPanel.Visibility = Visibility.Visible;
			UpdateUI();
		}

		private async Task RunRetryJobAsync()
		{
			// Force conservative strategy with double tool wear

			// Show job running state
			NoJobPanel.Visibility = Visibility.Collapsed;
			JobRunningPanel.Visibility = Visibility.Visible;
			ActiveJobTitle.Text = _game.ActiveJob.PartType + " (RETRY)";
			ActiveJobStrategy.Text = "Strategy: Conservative (FORCED)";

			// Run job with "ConservativeRetry" strategy
			await _game.StartJobAsync("ConservativeRetry");

			// Update progress bar
			while (_game.IsJobRunning)
			{
				JobProgressBar.Value = _game.JobProgress;
				JobProgressText.Text = $"{_game.JobProgress}%";
				await Task.Delay(100);
			}

			JobProgressBar.Value = 100;
			JobProgressText.Text = "100%";
			await Task.Delay(500);

			// Show result
			bool success = _game.LastJobSuccess;
			string result = success ? "✓ RETRY SUCCESS - Part accepted!" : "✗ RETRY FAILED - Part still out of spec";
			MessageBox.Show(result, "Retry Result");

			// If STILL failed, offer retry again (or limit retries?)
			if (!success && _game.CurrentRetryCount < 3) // Max 3 retries
			{
				await RunJobWithProgressAsync("Conservative"); // Recursive retry option
			}
			else
			{
				// Either succeeded or max retries reached
				var expenses = _game.CheckRecurringExpenses();
				if (expenses.Count > 0)
				{
					string expenseMessage = "The following expenses have been deducted:\n\n" +
										   string.Join("\n", expenses) +
										   $"\n\nNew balance: ${_game.CurrentPlayer.Money:F2}";
					MessageBox.Show(expenseMessage, "Recurring Expenses", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				_game.GenerateWorkOrders();
				JobRunningPanel.Visibility = Visibility.Collapsed;
				NoJobPanel.Visibility = Visibility.Visible;
				UpdateUI();
			}
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

			// NEW: Update stats
			JobsCompletedText.Text = _game.CurrentPlayer.JobsCompleted.ToString();
			JobsFailedText.Text = _game.CurrentPlayer.JobsFailed.ToString();

			// NEW: Update next unlock text
			UpdateNextUnlockText();
		}
		private void OpenToolShop_Click(object sender, RoutedEventArgs e)
		{
			if (_game.CurrentPlayer == null)
			{
				MessageBox.Show("Please start a new game first!", "No Active Game");
				return;
			}

			var toolShop = new Views.ToolShopWindow();
			toolShop.ShowDialog();
			UpdateUI();
		}

		private void UpdateNextUnlockText()
		{
			int completed = _game.CurrentPlayer.JobsCompleted;

			if (completed < 5)
			{
				NextUnlockText.Text = $"Next unlock: Bushing at {5 - completed} more jobs";
			}
			else if (completed < 10)
			{
				NextUnlockText.Text = $"Next unlock: Spacer at {10 - completed} more jobs";
			}
			else if (completed < 15)
			{
				NextUnlockText.Text = $"Next unlock: Fitting at {15 - completed} more jobs";
			}
			else if (completed < 20)
			{
				NextUnlockText.Text = $"Next unlock: Bracket at {20 - completed} more jobs";
			}
			else
			{
				NextUnlockText.Text = "All jobs unlocked! 🎉";
			}
		}
	}
}