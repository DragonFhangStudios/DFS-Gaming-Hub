using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics; // Needed for Process.Start
using DFS.Core.Models;
using DFS.Core.Services;

namespace DFS.WPFLauncher;

public partial class MainWindow : Window
{
	private readonly LauncherService _launcherService = new();

	public MainWindow()
	{
		InitializeComponent();
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		try
		{
			var games = _launcherService.GetInstalledGames();
			GameList.ItemsSource = games;

			if (games.Count == 0)
			{
				// Optional: Feedback if empty
				// MessageBox.Show("No games found in _Games folder!");
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Error loading games: {ex.Message}");
		}
	}

	private void LaunchGame_Click(object sender, RoutedEventArgs e)
	{
		// Use pattern matching to safely get the data context
		if (sender is Button button && button.DataContext is GameProject game)
		{
			if (File.Exists(game.ExePath))
			{
				Process.Start(new ProcessStartInfo(game.ExePath)
				{
					WorkingDirectory = game.FullPath
				});
			}
			else
			{
				MessageBox.Show($"Could not find executable at:\n{game.ExePath}", "Launch Error");
			}
		}
	}
}