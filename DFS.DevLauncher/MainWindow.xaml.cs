using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DFS.DevLauncher
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void LaunchPuzzle_Click(object sender, RoutedEventArgs e)
		{
			string puzzlePath = Path.GetFullPath(@"..\..\..\..\DFS.WPFLauncher\bin\Debug\net8.0-windows\DFS.WPFLauncher.exe");

			if (File.Exists(puzzlePath))
			{
				StatusLog.Text = "Launching Puzzle Framework...";
				Process.Start(puzzlePath);
			}
			else
			{
				StatusLog.Text = "❌ Puzzle Framework not found. Build it first!";
				MessageBox.Show("Puzzle Framework not found. Build it first!", "Launch Failed");
			}
		}

		private void LaunchSyndicate_Click(object sender, RoutedEventArgs e)
		{
			string syndicatePath = Path.GetFullPath(@"..\..\..\..\DFS.ProjectSyndicate\bin\Debug\net8.0-windows\DFS.ProjectSyndicate.exe");

			if (File.Exists(syndicatePath))
			{
				StatusLog.Text = "Launching Project Syndicate...";
				Process.Start(syndicatePath);
			}
			else
			{
				StatusLog.Text = "❌ Project Syndicate not found. Build it first!";
				MessageBox.Show("Project Syndicate not found. Build it first!", "Launch Failed");
			}
		}
	}
}
