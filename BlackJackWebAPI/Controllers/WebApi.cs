using BlackJackWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GameHistoryController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public GameHistoryController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET api/GameHistory/{userId}
		[HttpGet("{userId}")]
		public async Task<ActionResult<List<GameHistory>>> GetGameHistory(int userId)
		{
			var gameHistory = await _context.GameHistories
				.Where(g => g.UserId == userId)
				.Include(g => g.User)  
				.ToListAsync();

			if (gameHistory == null || gameHistory.Count == 0)
			{
				return NotFound("No game history found for the specified user.");
			}

			return Ok(gameHistory);
		}
	}
}