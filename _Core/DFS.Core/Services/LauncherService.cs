using System.IO;
using System.Collections.Generic;
using System.Linq; // Needed for specific list operations if we expand later
using DFS.Core.Models;

namespace DFS.Core.Services;

public class LauncherService
{
	public List<GameProject> GetInstalledGames()
	{
		var games = new List<GameProject>();

		// 1. Calculate the path to the '_Games' folder
		// We climb up 4 levels from the executable to get to the Repo Root
		string rootPath = Path.GetFullPath(
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "_Games")
		);

		// 2. Safety Check: Does the folder exist?
		if (Directory.Exists(rootPath))
		{
			// 3. Scan for directories
			var directories = Directory.GetDirectories(rootPath);

			foreach (var dir in directories)
			{
				var dirInfo = new DirectoryInfo(dir);

				// Filter out hidden folders like .git or .vs
				if (!dirInfo.Name.StartsWith("."))
				{
					// Clean up the name (e.g., "DFS.ProjectSyndicate" -> "Project Syndicate")
					string baseName = dirInfo.Name.Replace("DFS.", "").Replace(".", " ");
					// Regex split CamelCase
					string displayName = System.Text.RegularExpressions.Regex.Replace(baseName, "(\\B[A-Z])", " $1");

					games.Add(new GameProject(displayName, dirInfo.Name));
				}
			}
		}

		return games;
	}
}