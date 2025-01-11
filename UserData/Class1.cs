using Microsoft.EntityFrameworkCore;


namespace UserData
{
	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; } // Hashované heslo
		public decimal Balance { get; set; }
	}
	public class GameHistory
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string PlayerCards { get; set; }
		public string DealerCards { get; set; }
		public decimal Bet { get; set; }
		public string Result { get; set; }
		public DateTime PlayedAt { get; set; }

		public User User { get; set; }
	}


	public class BlackJackDB :DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<GameHistory> GameHistories { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = "server=localhost;database=blackjack;user=root;password=yourpassword";
			optionsBuilder.UseMySql("Server=localhost;Database=your_database_name;User ID=your_username;Password=your_password;",
			new MySqlServerVersion(new Version(8, 0, 21))
		}

	}
	public class AuthService
	{
		public bool Login(string username, string password)
		{
			using (var db = new BlackJackDB())
			{
				var user = db.Users.FirstOrDefault(u => u.Username == username);
				if (user != null)
				{
					return user.Password == password;
				}
				return false;
			}
		}
	}
}
