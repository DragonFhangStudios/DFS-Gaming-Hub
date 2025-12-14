using CNCMachinistSim.Models;
using CNCMachinistSim.Services;
using System.Windows;
using System.Windows.Controls;

namespace CNCMachinistSim.Views
{
	public partial class ToolShopWindow : Window
	{
		private GameManager _game;

		public ToolShopWindow()
		{
			InitializeComponent();
			_game = GameManager.Instance;
			RefreshDisplay();
		}

		private void RefreshDisplay()
		{
			// Update money
			MoneyDisplay.Text = $"${_game.CurrentPlayer.Money:F2}";

			// Update tools list
			ToolsList.ItemsSource = null;
			ToolsList.ItemsSource = _game.CurrentPlayer.OwnedTools;
		}

		private void ReplaceTool_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.Tag is Tool tool)
			{
				// Check if player can afford it
				if (!_game.CurrentPlayer.CanAfford(tool.ReplacementCost))
				{
					MessageBox.Show(
						$"Not enough money!\n\nYou need ${tool.ReplacementCost:F2} but only have ${_game.CurrentPlayer.Money:F2}",
						"Cannot Afford",
						MessageBoxButton.OK,
						MessageBoxImage.Warning
					);
					return;
				}

				// Confirm purchase (optional but nice UX)
				var result = MessageBox.Show(
					$"Replace {tool.Name}?\n\nCost: ${tool.ReplacementCost:F2}\nNew condition: 100%",
					"Confirm Purchase",
					MessageBoxButton.YesNo,
					MessageBoxImage.Question
				);

				if (result == MessageBoxResult.Yes)
				{
					// Charge player
					_game.CurrentPlayer.Charge(tool.ReplacementCost);

					// Reset tool to brand new
					tool.Condition = 100;

					// Refresh display
					RefreshDisplay();

					// Show success message
					MessageBox.Show(
						$"{tool.Name} replaced!\n\nNew balance: ${_game.CurrentPlayer.Money:F2}",
						"Tool Replaced",
						MessageBoxButton.OK,
						MessageBoxImage.Information
					);
				}
			}
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}