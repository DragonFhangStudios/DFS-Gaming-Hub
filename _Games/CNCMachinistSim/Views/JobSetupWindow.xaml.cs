using System.Windows;
using CNCMachinistSim.Models;
using System.Collections.Generic;
using System.Linq;

namespace CNCMachinistSim.Views
{
	public partial class JobSetupWindow : Window
	{
		public string SelectedStrategy { get; private set; }
		private WorkOrder _job;
		private List<Tool> _playerTools;

		public JobSetupWindow(WorkOrder job, List<Tool> playerTools)
		{
			InitializeComponent();

			_job = job;
			_playerTools = playerTools;

			// Display job info
			JobTitle.Text = job.PartType;
			JobPay.Text = $"Pay: ${job.BasePay:F2}";
			JobMaterial.Text = $"Material: ${job.MaterialCost:F2}";

			// Show required tools with their current condition
			var requiredTools = playerTools
				.Where(t => job.RequiredTools.Contains(t.Type))
				.ToList();

			RequiredToolsList.ItemsSource = requiredTools;
		}

		private void StartJob_Click(object sender, RoutedEventArgs e)
		{
			// Check if any required tools are broken
			var requiredTools = _playerTools
				.Where(t => _job.RequiredTools.Contains(t.Type))
				.ToList();

			// Check for broken tools (0% condition)
			var brokenTools = requiredTools.Where(t => t.IsBroken).ToList();
			if (brokenTools.Any())
			{
				string brokenList = string.Join(", ", brokenTools.Select(t => t.Name));
				MessageBox.Show(
					$"Cannot start job - broken tools must be replaced:\n\n{brokenList}",
					"Broken Tools",
					MessageBoxButton.OK,
					MessageBoxImage.Warning
				);
				return;
			}

			// OPTIONAL: Warn about low condition tools (but still allow)
			var wornTools = requiredTools.Where(t => t.Condition < 20 && !t.IsBroken).ToList();
			if (wornTools.Any())
			{
				string wornList = string.Join(", ", wornTools.Select(t => $"{t.Name} ({t.Condition}%)"));
				var result = MessageBox.Show(
					$"Warning - these tools are heavily worn:\n\n{wornList}\n\nProceed anyway? (High risk of failure)",
					"Worn Tools Warning",
					MessageBoxButton.YesNo,
					MessageBoxImage.Warning
				);

				if (result == MessageBoxResult.No)
					return;
			}

			// Determine selected strategy
			if (ConservativeRadio.IsChecked == true)
				SelectedStrategy = "Conservative";
			else if (NormalRadio.IsChecked == true)
				SelectedStrategy = "Normal";
			else if (AggressiveRadio.IsChecked == true)
				SelectedStrategy = "Aggressive";

			DialogResult = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}