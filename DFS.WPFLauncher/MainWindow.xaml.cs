using DFS.PuzzleFramework;
using DFS.Core;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows;

namespace DFS.WPFLauncher
{
	public partial class MainWindow : Window
	{
		private PuzzleManager _puzzleManager;
		private int rows = 6;
		private int cols = 6;

		public MainWindow()
		{
			InitializeComponent();
			_puzzleManager = new PuzzleManager(rows, cols);
			RenderBoard();
		}

		private void RenderBoard()
		{
			PuzzleGrid.Rows = rows;
			PuzzleGrid.Columns = cols;
			PuzzleGrid.Children.Clear();

			for (int r = 0; r < rows; r++)
			{
				for (int c = 0; c < cols; c++)
				{
					MatchTile tile = _puzzleManager.Board[r, c];

					var btn = new Button
					{
						Content = $"{tile.ColorType}\n{tile.ShapeType}",
						Margin = new Thickness(2),
						FontWeight = FontWeights.Bold,
						Background = Brushes.LightGray
					};

					PuzzleGrid.Children.Add(btn);
				}
			}
		}
	}

}