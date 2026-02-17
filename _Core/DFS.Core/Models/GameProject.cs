using System.IO;

namespace DFS.Core.Models
{
	public record GameProject(string Title, string DirectoryName)
	{
		// A helper to get the full path to the game folder based on where the Hub is running
		// We assume the Hub is in _Apps/DFS.WPFLauncher/bin/Debug/net8.0/
		public string FullPath => Path.GetFullPath(
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "_Games", DirectoryName)
		);

		// A helper to guess the EXE path (usually FolderName.exe)
		public string ExePath => Path.Combine(FullPath, $"{DirectoryName}.exe");

		public bool IsInstalled => Directory.Exists(FullPath);

		public string? CoverImagePath
		{
			get
			{
				if (!IsInstalled) return null;

				var coverPath = Path.Combine(FullPath, "cover.jpg");
				if (File.Exists(coverPath)) return coverPath;

				var headerPath = Path.Combine(FullPath, "header.png");
				if (File.Exists(headerPath)) return headerPath;

				return null;
			}
		}
	}
}