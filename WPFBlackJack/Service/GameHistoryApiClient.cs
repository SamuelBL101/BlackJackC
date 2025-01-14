using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using LibShared;
using Newtonsoft.Json;
using WebAPI.Controllers;
using GameHistory = LibShared.GameHistory;

namespace WPFBlackJack.Service
{
	/// <summary>
	/// Trieda reprezentujúca klienta pre prácu s API pre históriu hier a používateľské operácie.
	/// </summary>
	public class GameHistoryApiClient
	{
		private readonly HttpClient _httpClient;

		/// <summary>
		/// Konštruktor triedy inicializuje HttpClient, ktorý sa používa na komunikáciu s API.
		/// </summary>
		/// <param name="httpClient">HttpClient na vykonávanie HTTP požiadaviek.</param>
		public GameHistoryApiClient(HttpClient httpClient)
		{
			_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		/// <summary>
		/// Získava históriu hier pre daného používateľa podľa jeho ID.
		/// </summary>
		/// <param name="userId">ID používateľa, pre ktorého sa má získať história hier.</param>
		/// <returns>Zo zoznamu hier používateľa.</returns>
		public async Task<List<GameHistory>> GetGameHistoryAsync(int userId)
		{
			var response =
				await _httpClient.GetFromJsonAsync<List<GameHistory>>(
					$"https://localhost:7042/api/GameHistory/{userId}");
			return response ?? new List<GameHistory>();
		}

		/// <summary>
		/// Overuje prihlásenie používateľa na základe zadaného používateľského mena a hesla.
		/// </summary>
		/// <param name="username">Používateľské meno.</param>
		/// <param name="password">Heslo používateľa.</param>
		/// <returns>Objekt používateľa ak je prihlásenie úspešné, inak null.</returns>
		public async Task<User> AuthenticateUserAsync(string username, string password)
		{
			var loginData = new { Username = username, Password = password };
			var content = new StringContent(
				JsonConvert.SerializeObject(loginData),
				Encoding.UTF8,
				"application/json");

			var response = await _httpClient.PostAsync("https://localhost:7042/api/Login", content);

			if (response.IsSuccessStatusCode)
			{
				var user = await response.Content.ReadFromJsonAsync<User>();
				return user;
			}

			return null;
		}

		/// <summary>
		/// Získava informácie o používateľovi podľa jeho ID.
		/// </summary>
		/// <param name="userId">ID používateľa.</param>
		/// <returns>Objekt používateľa.</returns>
		public async Task<User> GetUserByIdAsync(int userId)
		{
			var response = await _httpClient.GetFromJsonAsync<User>($"https://localhost:7042/api/Login/{userId}");

			return response;
		}

		/// <summary>
		/// Aktualizuje zostatok používateľa v systéme.
		/// </summary>
		/// <param name="userId">ID používateľa, ktorého zostatok sa má aktualizovať.</param>
		/// <param name="newBalance">Nový zostatok používateľa.</param>
		/// <returns>Objekt reprezentujúci odpoveď na požiadavku.</returns>
		public async Task<object> UpdateUserBalanceAsync(int userId, int newBalance)
		{
			var updateRequest = new UpdateBalanceRequest
			{
				UserId = userId,
				NewBalance = newBalance
			};
			var content = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");
			var contentString = await content.ReadAsStringAsync();
			Console.WriteLine(contentString);

			var response = await _httpClient.PostAsync("https://localhost:7042/api/Login/updatebalance", content);

			return response;
		}

		/// <summary>
		/// Získava zostatok používateľa na základe jeho ID.
		/// </summary>
		/// <param name="userId">ID používateľa, pre ktorého sa má získať zostatok.</param>
		/// <returns>Zostatok používateľa.</returns>
		public async Task<int> GetUserBalanceAsync(int userId)
		{
			var response = await _httpClient.GetAsync($"https://localhost:7042/api/users/{userId}/balance");

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var balance = JsonConvert.DeserializeObject<int>(content); 

				return balance;
			}

			return 0;
		}

		/// <summary>
		/// Ukladá históriu hry do systému.
		/// </summary>
		/// <param name="gameHistory">Objekt reprezentujúci históriu hry.</param>
		/// <returns>Odpoveď na požiadavku o uložení histórie hry.</returns>
		public async Task<HttpResponseMessage> SaveGameHistoryAsync(GameHistory gameHistory)
		{
			var url = "https://localhost:7042/api/GameHistory";  
			var jsonContent = new StringContent(JsonConvert.SerializeObject(gameHistory), Encoding.UTF8, "application/json");
			Console.WriteLine(await jsonContent.ReadAsStringAsync());
			Console.WriteLine("Sending request with content: " + jsonContent);

			var response = await _httpClient.PostAsync(url, jsonContent);
			if (!response.IsSuccessStatusCode)
			{
				string responseContent = await response.Content.ReadAsStringAsync();
				Console.WriteLine("Status Code: " + response.StatusCode);
				Console.WriteLine("Response: " + responseContent);
			}
			else
			{
				Console.WriteLine("Game history saved successfully.");
			}
			return response;
		}
	}

}
