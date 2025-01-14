using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using LibShared;
using WPFBlackJack.Service;
using System.Net.Http;

namespace WPFBlackJack
{
	/// <summary>
	/// Interaction logic for WindowHra.xaml
	/// Táto trieda predstavuje hlavnú hernú obrazovku aplikácie Blackjack.
	/// Obsahuje logiku na spracovanie herných akcií, prácu s databázou používateľov, a aktualizáciu herného rozhrania.
	/// </summary>
	public partial class WindowHra : Window
	{
		private Blackjack _blackjack;
		private readonly GameHistoryApiClient _gameHistoryApiClient;

		private int idHraca = 1;

		/// <summary>
		/// Konštruktor okna, inicializuje hernú logiku, databázového klienta a herné rozhranie.
		/// </summary>
		/// <param name="blackjack">Inštancia hernej logiky</param>
		/// <param name="idHraca">ID aktuálneho hráča</param>
		/// <param name="gameHistoryApiClient">Klient na prístup k hernej histórii</param>
		public WindowHra(Blackjack blackjack, int idHraca, GameHistoryApiClient gameHistoryApiClient )
		{
			InitializeComponent();
			_blackjack = blackjack;
			DataContext = _blackjack;
			_gameHistoryApiClient = gameHistoryApiClient;
			ChooseBetTextBlock.Visibility = Visibility.Visible;
			idHraca = _blackjack.IdHraca;
			InitializeBalanceAsync();


		}

		/// <summary>
		/// Načíta počiatočný zostatok hráča z databázy a nastaví ho v hernej logike.
		/// </summary>
		private async void InitializeBalanceAsync()
		{
			var user = await _gameHistoryApiClient.GetUserByIdAsync(idHraca);
			_blackjack.HracBalance = (int)user.Balance;

		}

		/// <summary>
		/// Aktualizuje zostatok hráča v databáze.
		/// </summary>
		/// <param name="idHraca">ID hráča</param>
		/// <param name="newBalance">Nový zostatok</param>
		/// <returns>Indikácia, či aktualizácia prebehla úspešne</returns>
		public async Task<bool> UpdateUserBalanceAsync(int idHraca, int newBalance)
		{
			try
			{
				var updateRequest = new
				{
					UserId = idHraca,
					Balance = newBalance
				};

				var response = await _gameHistoryApiClient.UpdateUserBalanceAsync(idHraca,newBalance);

				if (response is HttpResponseMessage httpResponse && httpResponse.IsSuccessStatusCode)
				{
					return true;
				}
				else
				{
					MessageBox.Show("Failed to update balance.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					return false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while updating the balance: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}

		/// <summary>
		/// Spracováva kliknutie na tlačidlo "Start Game".
		/// Inicializuje hru a nastaví stávku.
		/// </summary>
		private void StartGameButton_Click(object sender, RoutedEventArgs e)
		{
			SecondHandCall.Visibility = Visibility.Collapsed;
			if (BetComboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int betAmount))
			{
				_blackjack.Rozdanie2(betAmount);
				_blackjack.AktualnaRuka = 0;
				ZobrazKartyPreRuky();
				ZobrazKarty(_blackjack.HracKarty[0], PlayerFirstHandPanel);
				ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel);
				ChooseBetTextBlock.Visibility = Visibility.Collapsed;
				AktualizujSplitTlačidlo();
				ZvysAktivnuRuku(); 

			}
			else
			{
				MessageBox.Show("Vyberte platnú hodnotu stávky!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

		}

		/// <summary>
		/// Zobrazí karty pre všetky ruky hráča.
		/// </summary>
		private void ZobrazKartyPreRuky()
		{
			ZobrazKarty(_blackjack.HracKarty[0], PlayerFirstHandPanel);
			PlayerFirstSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.HracKarty[0])}";
			DealerText.Visibility = Visibility.Visible;
			PlayerText.Visibility = Visibility.Visible;
			if (_blackjack.HracKarty.Count > 1)
			{
				PlayerSecondHandPanel.Visibility = Visibility.Visible;
				ZobrazKarty(_blackjack.HracKarty[1], PlayerSecondHandPanel);
				PlayerSecondSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.HracKarty[1])}";
			}
			else
			{
				PlayerSecondHandPanel.Visibility = Visibility.Collapsed;
				PlayerSecondSum.Text = string.Empty;
			}

		}

		/// <summary>
		/// Zobrazí karty v určenom panely WrapPanel.
		/// </summary>
		/// <param name="karty">Zoznam kariet na zobrazenie</param>
		/// <param name="panel">Cieľový panel</param>
		private void ZobrazKarty(List<Karta> karty, WrapPanel panel)
		{
			panel.Children.Clear();
			foreach (var karta in karty)
			{
				Image obrazokKarty = new Image
				{
					Source = new BitmapImage(new Uri(karta.Skryta ? "C:\\Users\\billy\\source\\repos\\UserData\\WPFBlackJack\\Images\\back.png" : karta.Obrazok, UriKind.Absolute)),
					Width = 100, 
					Height = 150, 
					Margin = new Thickness(5)
				};

				panel.Children.Add(obrazokKarty);
			}

			if (panel == PlayerFirstHandPanel)
			{
				PlayerFirstSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.HracKarty[0])}";
			}
			else if (panel == DealerCardsPanel)
			{
				DealerSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.DealerKarty)}";
			}

		}

		/// <summary>
		/// Spracováva kliknutie na tlačidlo "Hit".
		/// Pridá kartu do aktuálnej ruky a kontroluje, či ruka neprekročila hodnotu 21.
		/// </summary>
		private void HitButton_Click(object sender, RoutedEventArgs e)
		{
				_blackjack.Hit(_blackjack.AktualnaRuka);
				ZobrazKartyPreRuky();

				if (_blackjack.SumaKariet(_blackjack.HracKarty[_blackjack.AktualnaRuka]) > 21)
				{
					MessageBox.Show($"Hand {_blackjack.AktualnaRuka + 1} busted!", "Game Result", MessageBoxButton.OK, MessageBoxImage.Warning);
					StandButton_Click(sender, e); 
				}
			

		}

		/// <summary>
		/// Metóda obsluhuje kliknutie na tlačidlo "Stand".
		/// Ak má hráč viac ako jednu ruku, prejde na ďalšiu ruku. 
		/// Inak vykoná ťahanie kariet pre dealera a vyhodnotí výsledok hry.
		/// </summary>
		private void StandButton_Click(object sender, RoutedEventArgs e)
		{
			// Kontrola, či má hráč viacero rúk a či ešte nehral všetky
			if (_blackjack.HracKarty.Count > 1 && _blackjack.AktualnaRuka < _blackjack.HracKarty.Count - 1)
			{
				_blackjack.AktualnaRuka++;
				ZobrazKartyPreRuky(); 
				ZvysAktivnuRuku(); 

				return; 
			}

			// Dealer ťahá karty, kým súčet jeho kariet je menší ako 17
			while (_blackjack.SumaKariet(_blackjack.DealerKarty) < 17)
			{
				_blackjack.DealerKarty.Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));
				_blackjack.DealerKarty[1].Skryta = false;
				ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel); 
			}
			_blackjack.DealerKarty[1].Skryta = false;
			ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel); 

			VyhodnotHru();

			BalanceText.Text = $" {_blackjack.HracBalance}";

			UkonciHruAsync();

		}

		/// <summary>
		/// Vyhodnotí výsledok hry na základe súčtov kariet hráča a dealera.
		/// </summary>
		private void VyhodnotHru()
		{
			int dealerSum = _blackjack.SumaKariet(_blackjack.DealerKarty);

			foreach (var ruka in _blackjack.HracKarty)
			{
				int playerSum = _blackjack.SumaKariet(ruka);

				if (playerSum > 21)
				{
					MessageBox.Show("You busted! Dealer wins.", "Game Result", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else if (dealerSum > 21 || playerSum > dealerSum)
				{
					MessageBox.Show("You win!", "Game Result", MessageBoxButton.OK, MessageBoxImage.Information);
					_blackjack.HracBalance += _blackjack.AktualnaStavka * 2;
				}
				else if (playerSum == dealerSum)
				{
					MessageBox.Show("It's a tie!", "Game Result", MessageBoxButton.OK, MessageBoxImage.Information);
					_blackjack.HracBalance += _blackjack.AktualnaStavka;
				}
				else
				{
					MessageBox.Show("Dealer wins!", "Game Result", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				_ = UpdateUserBalanceAsync(idHraca, _blackjack.HracBalance);
				InitializeBalanceAsync();
			}
		}

		/// <summary>
		/// Asynchrónne ukončí hru, resetuje stav a uloží históriu hry.
		/// </summary>
		private async Task UkonciHruAsync()
		{
			string playerCards = string.Join(", ", _blackjack.HracKarty
				.SelectMany(hand => hand.Select(card => card.Hodnota)));  
			string dealerCards = string.Join(", ", _blackjack.DealerKarty.Select(card => card.Hodnota));  
			decimal bet = _blackjack.AktualnaStavka;  
			string result = _blackjack.HracBalance > 0 ? "win" : _blackjack.HracBalance < 0 ? "lose" : "draw";  

			await SaveGameHistoryAsync(playerCards, dealerCards, bet, result);

			_blackjack.Reset();

			PlayerFirstHandPanel.Children.Clear();
			DealerCardsPanel.Children.Clear();
			PlayerSecondHandPanel.Children.Clear();

			PlayerFirstSum.Text = string.Empty;
			DealerSum.Text = string.Empty;
			PlayerSecondSum.Text = string.Empty;
			PlayerSecondSum.Visibility = Visibility.Collapsed;
			DealerText.Visibility = Visibility.Collapsed;
			PlayerText.Visibility = Visibility.Collapsed;
			ChooseBetTextBlock.Visibility = Visibility.Visible;
			AktualizujSplitTlačidlo();

		}

		/// <summary>
		/// Uloží históriu hry na server.
		/// </summary>
		private async Task SaveGameHistoryAsync(string playerCards, string dealerCards, decimal bet, string result)
		{
			var gameHistory = new GameHistory
			{
				UserId = _blackjack.IdHraca,
				PlayerCards = playerCards,  
				DealerCards = dealerCards,  
				Bet = bet,
				Result = result,
				PlayedAt = DateTime.Now
			};

			
			var response = await _gameHistoryApiClient.SaveGameHistoryAsync(gameHistory);

			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Game history saved successfully.");
			}
			else
			{
				Console.WriteLine("Failed to save game history.");
			}
		}

		/// <summary>
		/// Obsluhuje kliknutie na tlačidlo "Split".
		/// Rozdelí ruku hráča, ak obsahuje pár, a aktualizuje zobrazenie.
		/// </summary>
		private void SplitButton_Click(object sender, RoutedEventArgs e)
		{
			if (_blackjack.JeParPrvejRuky())
			{
				SplitHand();
				UpdateHandLayoutForSplit();
				//ZobrazKartyPreRuky();
				ZobrazKarty(_blackjack.HracKarty[0], PlayerFirstHandPanel);
				ZobrazKarty(_blackjack.HracKarty[1], PlayerSecondHandPanel);
				ZvysAktivnuRuku(); 
				AktualizujSplitTlačidlo();
			}
		}

		/// <summary>
		/// Rozdelí aktuálnu ruku hráča na dve samostatné ruky.
		/// </summary>
		private void SplitHand()
		{
			Debug.WriteLine("Splitting hand...");

			var druhaRuka = new List<Karta> { _blackjack.HracKarty[0][1] };
			Debug.WriteLine($"Card to move: {_blackjack.HracKarty[0][1].Hodnota}");

			_blackjack.HracKarty[0].RemoveAt(1);
			_blackjack.HracKarty.Add(druhaRuka);

			_blackjack.HracKarty[0].Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));
			_blackjack.HracKarty[1].Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));
			PlayerSecondSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.HracKarty[1])}";

		}

		/// <summary>
		/// Aktualizuje rozloženie panelov po rozdelení ruky.
		/// </summary>
		private void UpdateHandLayoutForSplit()
		{
			PlayerFirstHandPanel.HorizontalAlignment = HorizontalAlignment.Left;
			PlayerSecondHandPanel.Visibility = Visibility.Visible;
			SecondHandCall.Visibility = Visibility.Visible;
			PlayerSecondHandPanel.HorizontalAlignment = HorizontalAlignment.Right;
			PlayerSecondSum.Visibility = Visibility.Visible;
		}

		/// <summary>
		/// Aktualizuje tlačidlo Split podľa toho, či je možné rozdeliť ruku.
		/// </summary>
		private void AktualizujSplitTlačidlo()
		{
			SplitButton.Visibility = _blackjack.JeParPrvejRuky() ? Visibility.Visible : Visibility.Collapsed;

		}

		/// <summary>
		/// Zvýrazní aktuálnu ruku hráča.
		/// </summary>
		private void ZvysAktivnuRuku()
		{
			if (_blackjack.AktualnaRuka == 0)
			{
				PlayerFirstHandPanel.Opacity = 1.0; 
				PlayerSecondHandPanel.Opacity = 0.5; 
			}
			else if (_blackjack.AktualnaRuka == 1)
			{
				PlayerFirstHandPanel.Opacity = 0.5; 
				PlayerSecondHandPanel.Opacity = 1.0; 
			}
		}

	}
}
