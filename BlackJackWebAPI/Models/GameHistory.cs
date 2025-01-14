namespace BlackJackWebAPI.Models
{
	public class GameHistoryd
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public string GameName { get; set; }
		public DateTime PlayedOn { get; set; }
		public int Score { get; set; }
	}
}
