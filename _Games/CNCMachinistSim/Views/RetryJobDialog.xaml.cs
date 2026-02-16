using System.Windows;
using CNCMachinistSim.Models;

namespace CNCMachinistSim.Views
{
	public partial class RetryJobDialog : Window
	{
		public bool ShouldRetry { get; private set; }

		public RetryJobDialog(WorkOrder failedJob, int retryCount)
		{
			InitializeComponent();

			// Display job info
			JobTitle.Text = $"{failedJob.PartType} - Out of Tolerance";

			// Calculate costs
			decimal scrapValue = failedJob.MaterialCost * 0.5m;
			decimal doubleMaterialCost = failedJob.MaterialCost * 2m;
			decimal netCost = doubleMaterialCost - scrapValue;

			// Update UI
			MaterialCostText.Text = $"DOUBLE material cost: ${doubleMaterialCost:F2}";
			RetryCountText.Text = $"#{retryCount}";
			ScrapValueText.Text = $"+${scrapValue:F2}";
			NewMaterialCostText.Text = $"-${doubleMaterialCost:F2}";
			NetCostText.Text = $"-${netCost:F2}";
		}

		private void RetryJob_Click(object sender, RoutedEventArgs e)
		{
			ShouldRetry = true;
			DialogResult = true;
			Close();
		}

		private void TakeLoss_Click(object sender, RoutedEventArgs e)
		{
			ShouldRetry = false;
			DialogResult = true;
			Close();
		}
	}
}