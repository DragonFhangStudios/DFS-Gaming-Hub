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
			RefreshToolsList();
		}

		private void RefreshToolsList()
		{
			ToolsList.ItemsSource = null;
			ToolsList.ItemsSource = _game.CurrentPlayer.OwnedTools;
		}

		private void ReplaceTool_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.Tag is Tool tool)
			{
				if (_game.CurrentPlayer.CanAfford(tool.ReplacementCost))
				{
					_game.CurrentPlayer.Charge(tool.ReplacementCost);
					tool.Condition = 100; // Reset to new
					RefreshToolsList();
					MessageBox.Show($"Tool replaced! -{tool.ReplacementCost:C}");
				}
				else
				{
					MessageBox.Show("Not enough money!", "Cannot Afford");
				}
			}
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}