using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WPFBlackJack
{
	/// <summary>
	/// Interaction logic for WindowHra.xaml
	/// </summary>
	public partial class WindowHra : Window
	{
		private Blackjack _blackjack;

		public WindowHra()
		{
			InitializeComponent();
			_blackjack = new Blackjack();
			DataContext = _blackjack;

		}

		private void StartGameButton_Click(object sender, RoutedEventArgs e)
		{
			if (int.TryParse(BetTextBox.Text, out int betAmount))
			{
				_blackjack.Rozdanie(betAmount);
			}
			else
			{
				MessageBox.Show("Zadajte platnú hodnotu stávky!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			ZobrazKarty(_blackjack.HracKarty, PlayerCardsPanel);
			ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel);


		}
		private void ZobrazKarty(List<Karta> karty, WrapPanel panel)
		{
			panel.Children.Clear();
			foreach (var karta in karty)
			{
				Image obrazokKarty = new Image
				{
					Source = new BitmapImage(new Uri(karta.Obrazok, UriKind.Absolute)),
					Width = 100, 
					Height = 150, 
					Margin = new Thickness(5)
				};

				panel.Children.Add(obrazokKarty);
			}

			if (panel == PlayerCardsPanel)
			{
				PlayerSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.HracKarty)}";
			}
			else if (panel == DealerCardsPanel)
			{
				DealerSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.DealerKarty)}";
			}
		}



		private void HitButton_Click(object sender, RoutedEventArgs e)
		{
			_blackjack.Hit();
			ZobrazKarty(_blackjack.HracKarty, PlayerCardsPanel);
			if (_blackjack.SumaKariet(_blackjack.HracKarty) > 21)
			{
				MessageBox.Show("Too many! Prehral si.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Warning);

				UkoncitHru();
			}
		}

		private void UkoncitHru()
		{
			_blackjack.Reset();
			PlayerCardsPanel.Children.Clear();
			DealerCardsPanel.Children.Clear();
			PlayerSum.Text = "Suma: 0";
			DealerSum.Text = "Suma: 0";

		}

		private void StandButton_Click(object sender, RoutedEventArgs e)
		{
			while (_blackjack.SumaKariet(_blackjack.DealerKarty) < 17)
			{
				_blackjack.DealerKarty.Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));
				ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel);
			}

			// Získanie súčtov hráča a dealera
			int playerSum = _blackjack.SumaKariet(_blackjack.HracKarty);
			int dealerSum = _blackjack.SumaKariet(_blackjack.DealerKarty);

			// Výsledok hry
			if (dealerSum > 21 || playerSum > dealerSum)
			{
				MessageBox.Show("You win!", "Game Result", MessageBoxButton.OK, MessageBoxImage.Information);
				_blackjack.HracBalance += _blackjack.AktualnaStavka * 2; // Vyhra = dvojnásobok stávky
			}
			else if (playerSum == dealerSum)
			{
				MessageBox.Show("It's a tie!", "Game Result", MessageBoxButton.OK, MessageBoxImage.Information);
				_blackjack.HracBalance += _blackjack.AktualnaStavka; // Remíza = vrátenie stávky
			}
			else
			{
				MessageBox.Show("Dealer wins!", "Game Result", MessageBoxButton.OK, MessageBoxImage.Information);
			}

			// Aktualizácia balancie
			BalanceText.Text = $"Balance: {_blackjack.HracBalance}";

			// Reset hry
			_blackjack.Reset();
			PlayerCardsPanel.Children.Clear();
			DealerCardsPanel.Children.Clear();
			PlayerSum.Text = string.Empty;
			DealerSum.Text = string.Empty;
		}
	}
}
