using Microsoft.EntityFrameworkCore;
using System;

namespace LibShared // You can adjust this namespace based on your project structure
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<GameHistory> GameHistories { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySql("Server=localhost;Database=BlackJack;User=root;Password=Parkovisko3@;",
				new MySqlServerVersion(new Version(8, 0, 21)));
		}
	}

	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
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
}