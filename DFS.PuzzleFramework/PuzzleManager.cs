using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DFS.Core;
using DFS.Core.Enums;

namespace DFS.PuzzleFramework
{
	public class PuzzleManager
	{
		public MatchTile[,] Board { get; private set; }
		public int Rows { get; }
		public int Columns { get; }

		public PuzzleManager(int rows, int columns)
		{
			Rows = rows;
			Columns = columns;
			Board = new MatchTile[rows, columns];
			GenerateRandomBoard();
		}

		private void GenerateRandomBoard()
		{
			var rand = new Random();
			var colors = Enum.GetValues(typeof(TileType));

			for (int r = 0; r < Rows; r++)
			{
				for (int c = 0; c < Columns; c++)
				{
					// Use just color types for now
					var color = (TileType)colors.GetValue(rand.Next(0, 4)); // Red, Blue, Green, Yellow
					var shape = (TileType)colors.GetValue(rand.Next(4, 8)); // Circle, Square, etc.
					Board[r, c] = new MatchTile(color, shape);
				}
			}
		}

		public List<(int row, int col)> FindColorMatches(int matchCount = 3)
		{
			var matched = new List<(int, int)>();

			// Horizontal checks
			for (int r = 0; r < Rows; r++)
			{
				int streak = 1;
				for (int c = 1; c < Columns; c++)
				{
					if (Board[r, c].ColorType == Board[r, c - 1].ColorType)
					{
						streak++;
						if (streak >= matchCount && c == Columns - 1)
						{
							for (int i = 0; i < streak; i++)
								matched.Add((r, c - i));
						}
					}
					else
					{
						if (streak >= matchCount)
						{
							for (int i = 1; i <= streak; i++)
								matched.Add((r, c - i));
						}
						streak = 1;
					}
				}
			}

			return matched;
		}
	}
}
