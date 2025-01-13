using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFBlackJack;
namespace WPFBlackJack
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		public MainWindow()
		{
			SQLitePCL.Batteries.Init();
			InitializeComponent();
			InitializeDatabase();
			
		}

		private void InitializeDatabase()
		{
			using (var context = new ApplicationDbContext())
			{
				context.Database.EnsureCreated();
			}
		}
	


		private void ClearPlaceholder(object sender, RoutedEventArgs e)
		{
			if (sender is TextBox textBox && textBox.Text == "Používateľské meno")
			{
				textBox.Text = "";
				textBox.Foreground = Brushes.Black;
			}
		}

		private async void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			string username = UsernameTextBox.Text;
			string password = PasswordBox.Password;
			username = "testuser";
			password = "password123";
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				MessageBox.Show("Please enter both username and password.");
				return;
			}

			using (var context = new ApplicationDbContext())
			{
				var user = context.Users
					.FirstOrDefault(u => u.Username == username && u.Password == password);
				if (user != null)
				{
					
					var daco = new Blackjack(user.Balance, user.Id);
					var nextWindow = new WindowHra(daco, user.Id);
					nextWindow.Show();
					this.Close();
				}
				else
				{
					MessageBox.Show("Invalid username or password.");
				}
			}
		}
		/**
		private async Task LoadGameHistory(int userId)
		{
			try
			{

				var response = await _httpClient.GetAsync($"gamehistory/{userId}");
				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					var gameHistory = JsonSerializer.Deserialize<List<GameHistory>>(responseBody);
					//var gameHistory = await _gameHistoryApiClient.GetGameHistoryAsync(userId);
					foreach (var game in gameHistory)
					{
						Console.WriteLine(game.Id);  // Example, adjust as needed
					}
					if (gameHistory.Any())
					{
						// Otvoríme okno na zobrazenie histórie hier
						var historyWindow = new GameHistoryWindow(gameHistory);
						historyWindow.Show();
						this.Close();
					}
					else
					{
						MessageBox.Show("Žiadna história hier nebola nájdená.");
					}
				}
				else
				{
					MessageBox.Show("Nepodarilo sa načítať históriu hier.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Chyba pri načítavaní histórie: {ex.Message}");
			}
		}
		**/

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

		private void AddUserButton_Click(object sender, RoutedEventArgs e)
		{
			using (var context = new ApplicationDbContext())
			{
				var newUser = new User
				{
					Username = "a",
					Password = "a",
					Balance = 1000.00m
				};

				context.Users.Add(newUser);
				context.SaveChanges();  
			}
		}

		private void LoadUsersButton_Click(object sender, RoutedEventArgs e)
		{
			using (var context = new ApplicationDbContext())
			{
				var users = context.Users.ToList();  

			}
		}

		private void ShowGameHistoryCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private void ShowGameHistoryCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}