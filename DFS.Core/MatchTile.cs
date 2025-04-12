using DFS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFS.Core
{
	public class MatchTile
	{
		public TileType ColorType { get; set; }
		public TileType ShapeType { get; set; }
		public bool IsMatched { get; set; } = false;

		public MatchTile(TileType color, TileType shape)
		{
			ColorType = color;
			ShapeType = shape;
		}
	}
}
