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
			if (BetComboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int betAmount))
			{
				_blackjack.Rozdanie(betAmount);

				// Zobrazenie kariet hráča (prvý balík) a dealera
				ZobrazKarty(_blackjack.HracKarty[0], PlayerCardsPanel);
				ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel);

				// Aktualizácia tlačidla Split
				AktualizujSplitTlačidlo();
			}
			else
			{
				MessageBox.Show("Vyberte platnú hodnotu stávky!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

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
				PlayerSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.HracKarty[0])}";
			}
			else if (panel == DealerCardsPanel)
			{
				DealerSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.DealerKarty)}";
			}
		}



		private void HitButton_Click(object sender, RoutedEventArgs e)
		{
			_blackjack.Hit(0);
			ZobrazKarty(_blackjack.HracKarty[0], PlayerCardsPanel);
			AktualizujSplitTlačidlo();

			if (_blackjack.SumaKariet(_blackjack.HracKarty[0]) > 21)
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
			foreach (var ruka in _blackjack.HracKarty)
			{
				// Dealer ťahá karty, kým nemá minimálne 17
				while (_blackjack.SumaKariet(_blackjack.DealerKarty) < 17)
				{
					_blackjack.DealerKarty.Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));
					ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel);
				}

				// Porovnanie výsledkov pre každú ruku
				int playerSum = _blackjack.SumaKariet(ruka);
				int dealerSum = _blackjack.SumaKariet(_blackjack.DealerKarty);

				if (dealerSum > 21 || playerSum > dealerSum)
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
			}

			BalanceText.Text = $"Balance: {_blackjack.HracBalance}";

			_blackjack.Reset();
			PlayerCardsPanel.Children.Clear();
			DealerCardsPanel.Children.Clear();
			PlayerSecondHandPanel.Children.Clear();
			PlayerSum.Text = string.Empty;
			DealerSum.Text = string.Empty;
		}

		private void SplitButton_Click(object sender, RoutedEventArgs e)
		{
			if (_blackjack.JeParPrvejRuky())
			{
				// Rozdelenie prvej ruky na dve ruky
				var druhaRuka = new List<Karta> { _blackjack.HracKarty[0][1] };
				_blackjack.HracKarty[0].RemoveAt(1);
				_blackjack.HracKarty.Add(druhaRuka);

				// Rozdanie novej karty každej ruke
				_blackjack.HracKarty[0].Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));
				_blackjack.HracKarty[1].Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));

				UpdateHandLayoutForSplit();

				// Zobrazenie oboch rúk
				ZobrazKarty(_blackjack.HracKarty[0], PlayerCardsPanel);
				ZobrazKarty(_blackjack.HracKarty[1], PlayerSecondHandPanel);

				// Skrytie tlačidla Split
				SplitButton.Visibility = Visibility.Collapsed;
			}
		}

		private void UpdateHandLayoutForSplit()
		{
			PlayerCardsPanel.HorizontalAlignment = HorizontalAlignment.Left;
			PlayerSecondHandPanel.Visibility = Visibility.Visible;
			PlayerSecondHandPanel.HorizontalAlignment = HorizontalAlignment.Right;
			PlayerSum2.Visibility = Visibility.Visible;
		}

		private void AktualizujSplitTlačidlo()
		{
			if (_blackjack.JeParPrvejRuky())
			{
				SplitButton.Visibility = Visibility.Visible;
			}
			else
			{
				SplitButton.Visibility = Visibility.Collapsed;
			}
		}

	}
}
