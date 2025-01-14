using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BlackJackWebAPI.Models;
using LibShared;
using Newtonsoft.Json; 

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public LoginController(ApplicationDbContext context)
		{
			_context = context;
		}

		// POST api/Login
		[HttpPost]
		public async Task<ActionResult<User>> Login([FromBody] LoginRequest loginRequest)
		{

			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

			if (user == null)
			{
				return Unauthorized("Invalid username or password.");
			}


			if (user.Password != loginRequest.Password)
			{
				return Unauthorized("Invalid username or password.");
			}

			return Ok(user); 
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUserById(int id)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			return Ok(user);
		}

		[HttpPost("updatebalance")]
		public async Task<ActionResult<User>> UpdateBalance([FromBody] UpdateBalanceRequest updateRequest)
		{

			Console.WriteLine($"Received UpdateBalanceRequest: UserId = {updateRequest.UserId}, NewBalance = {updateRequest.NewBalance}");

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == updateRequest.UserId);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			user.Balance = updateRequest.NewBalance;

			await _context.SaveChangesAsync();

			return Ok(user); 
		}
		[HttpGet("{id}/balance")]
		public async Task<ActionResult<int>> GetUserBalance(int id)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

			if (user == null)
			{
				return NotFound("User not found.");
			}

			return Ok(user.Balance);
		}
		[HttpGet("{id}/games")]
		public async Task<ActionResult<List<GameHistory>>> GetGameHistory(int id)
		{
			var gameHistory = await _context.GameHistories
				.Where(g => g.UserId == id)
				.ToListAsync();
			if (gameHistory == null || gameHistory.Count == 0)
			{
				return NotFound("No game history found for the specified user.");
			}
			return Ok(gameHistory);
		}
	}

	public class LoginRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
	public class UpdateBalanceRequest
	{
		[JsonPropertyName("UserId")]
		public int UserId { get; set; }

		[JsonPropertyName("NewBalance")]
		public int NewBalance { get; set; }
	}
	
}