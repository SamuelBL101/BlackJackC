using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibShared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using MySqlConnector;
using WPFBlackJack;
using WPFBlackJack.Service;
namespace WPFBlackJack
{
	/// <summary>
	/// Hlavné okno aplikácie pre hru Blackjack.
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly GameHistoryApiClient _gameHistoryApiClient;

		/// <summary>
		/// Konštruktor, ktorý inicializuje komponenty a inicializuje klienta pre API histórie hier.
		/// </summary>
		public MainWindow()
		{
			SQLitePCL.Batteries.Init();
			InitializeComponent();
			

			var httpClient = new HttpClient();
			_gameHistoryApiClient = new GameHistoryApiClient(httpClient);

		}

		/// <summary>
		/// Vymaže text v textovom poli, keď je text rovný placeholderu "Používateľské meno".
		/// </summary>
		private void ClearPlaceholder(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox textBox && textBox.Text == "Používateľské meno")
			{
				textBox.Text = "";
				textBox.Foreground = Brushes.Black;
			}
		}

		/// <summary>
		/// Odeslanie prihlasovacích údajov a autentifikácia používateľa.
		/// </summary>
		private async void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			string username = UsernameTextBox.Text;
			string password = PasswordBox.Password;

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				MessageBox.Show("Please enter both username and password.");
				return;
			}

			var user = await _gameHistoryApiClient.AuthenticateUserAsync(username, password);

			if (user != null)
			{
				if (LoadGameHistoryCheckBox.IsChecked == true)
				{
					LoadGameHistory(user.Id);
				}
				else
				{
					var blackjack = new Blackjack(user.Balance, user.Id);
					var windowHra = new WindowHra(blackjack, user.Id, _gameHistoryApiClient);  
					windowHra.Show();
					this.Close();  
				}
				
			}
			else
			{
				MessageBox.Show("Invalid username or password.");
			}
		}

		/// <summary>
		/// Načíta históriu hier používateľa.
		/// </summary>
		private async void LoadGameHistory(int id)
		{
			var gameHistory = await _gameHistoryApiClient.GetGameHistoryAsync(id);

			if (gameHistory != null)
			{
				GameHistoryWindow historyWindow = new GameHistoryWindow(gameHistory);
				historyWindow.Show();
			}
			else
			{
				MessageBox.Show("Neexistujú žiadne záznamy o histórii hier.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}


		private void SetPlaceholder(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
			{
				textBox.Text = "Používateľské meno";
				textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
			}
		}

		private void ClearPasswordPlaceholder(object sender, RoutedEventArgs e)
		{
			if (sender is PasswordBox passwordBox && passwordBox.Password == "Heslo")
			{
				passwordBox.Password = "";
				passwordBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
			}
		}

		private void SetPasswordPlaceholder(object sender, RoutedEventArgs e)
		{
			if (sender is PasswordBox passwordBox && string.IsNullOrWhiteSpace(passwordBox.Password))
			{
				passwordBox.Password = "Heslo";
				passwordBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
			}
		}
	}
}