using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SharedLibDB
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<GameHistory> GameHistories { get; set; }

		// Konstruktor na inicializáciu DbContext s pripojením k SQLite alebo In-Memory databáze
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		// Definovanie konfigurácie databázy (napr. SQLite alebo in-memory)
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Data Source=shared_database.db"); // pre SQLite
			// Alebo pre in-memory:
			// optionsBuilder.UseInMemoryDatabase("SharedDb");
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
