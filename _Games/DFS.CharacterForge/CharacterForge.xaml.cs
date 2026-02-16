using System;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;

namespace DFS.CharacterForge
{
	public partial class CharacterForge : Window
	{
		private readonly string[] CursedItems = new[]
		{
			"Rusty Dog Tags",
			"Cracked Radiation Pendant",
			"Bloodied Watch",
			"Sealed Vial with Black Liquid",
			"Bunker Access Token (Revoked)"
		};

		private readonly string[] Secrets = new[]
		{
			"Once betrayed a brother-in-arms.",
			"Hears voices when near the ruins.",
			"Smuggled government rations for rebels.",
			"Survived a forbidden zone—shouldn't be alive.",
			"Was experimented on during early outbreaks."
		};

		public CharacterForge()
		{
			InitializeComponent();
		}

		private void CharacterForge_Loaded(object sender, RoutedEventArgs e)
		{
			GenerateFlavor();
		}

		private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			GenerateFlavor();
		}

		private void GenerateButton_Click(object sender, RoutedEventArgs e)
		{
			GenerateFlavor();
		}

		private void GenerateFlavor()
		{
			if (CursedItemBox == null || SecretBox == null)
			{
				MessageBox.Show("UI controls not initialized yet!");
				return;
			}

			CursedItemBox.Text = CursedItems[RandomNumberGenerator.GetInt32(CursedItems.Length)];
			SecretBox.Text = Secrets[RandomNumberGenerator.GetInt32(Secrets.Length)];

			// Optional: Append to log
			BackstoryBox.AppendText($"Cursed Item: {CursedItemBox.Text}\n");
			BackstoryBox.AppendText($"Secret: {SecretBox.Text}\n");
			BackstoryBox.AppendText("====================================\n");
			BackstoryBox.ScrollToEnd();
		}
	}
}
