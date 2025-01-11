using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

		private void LoginButton_Click(object sender, RoutedEventArgs e)
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

					var nextWindow = new WindowHra();
					nextWindow.Show();
					this.Close();
				}
				else
				{
					MessageBox.Show("Invalid username or password.");
				}
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
	}
}