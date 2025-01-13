using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace WPFBlackJack.Service
{

	internal class GameHistoryApiClient
	{
		private readonly HttpClient _httpClient;

		public GameHistoryApiClient(HttpClient httpClient)
		{
			_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		public async Task<List<GameHistory>> GetGameHistoryAsync(int userId)
		{
			var response = await _httpClient.GetFromJsonAsync<List<GameHistory>>($"https://localhost:7042/api/GameHistory/{userId}");
			return response ?? new List<GameHistory>();
		}
	}
}
