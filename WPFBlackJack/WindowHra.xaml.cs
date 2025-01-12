using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;

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
			SecondHandCall.Visibility = Visibility.Collapsed;
			if (BetComboBox.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Content.ToString(), out int betAmount))
			{
				_blackjack.Rozdanie2(betAmount);
				_blackjack.AktualnaRuka = 0;
				ZobrazKarty(_blackjack.HracKarty[0], PlayerFirstHandPanel);
				ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel);

				AktualizujSplitTlačidlo();
			}
			else
			{
				MessageBox.Show("Vyberte platnú hodnotu stávky!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
			}

		}
		private void ZobrazKartyPreRuky()
		{
			ZobrazKarty(_blackjack.HracKarty[0], PlayerFirstHandPanel);
			PlayerFirstSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.HracKarty[0])}";

			// Druhá ruka (ak existuje)
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

			if (panel == PlayerFirstHandPanel)
			{
				PlayerFirstSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.HracKarty[0])}";
			}
			else if (panel == DealerCardsPanel)
			{
				DealerSum.Text = $"Suma: {_blackjack.SumaKariet(_blackjack.DealerKarty)}";
			}

		}



		private void HitButton_Click(object sender, RoutedEventArgs e)
		{
			if (_blackjack.AktualnaRuka < _blackjack.HracKarty.Count)
			{
				_blackjack.HracKarty[_blackjack.AktualnaRuka].Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));
				ZobrazKartyPreRuky();

				if (_blackjack.SumaKariet(_blackjack.HracKarty[_blackjack.AktualnaRuka]) > 21)
				{
					MessageBox.Show($"Hand {_blackjack.AktualnaRuka + 1} busted!", "Game Result", MessageBoxButton.OK, MessageBoxImage.Warning);
					StandButton_Click(sender, e); 
				}
			}

		}

		private void AktualizujSuma(int indexRuky, TextBlock sumaTextBlock)
		{
			var suma = _blackjack.SumaKariet(_blackjack.HracKarty[indexRuky]);
			sumaTextBlock.Text = $"Sum: {suma}";
		}

		private void UkoncitHru()
		{
			_blackjack.Reset();
			PlayerFirstHandPanel.Children.Clear();
			DealerCardsPanel.Children.Clear();
			PlayerSecondHandPanel.Children.Clear();
			PlayerSecondSum.Text = "Suma: 0";
			PlayerFirstSum.Text = "Suma: 0";
			DealerSum.Text = "Suma: 0";

		}

		private void StandButton_Click(object sender, RoutedEventArgs e)
		{
			if (_blackjack.HracKarty.Count > 1)
			{
				_blackjack.AktualnaRuka++;

				if (_blackjack.AktualnaRuka < _blackjack.HracKarty.Count)
				{
					ZobrazKartyPreRuky();
					return;
				}
			}
			while (_blackjack.SumaKariet(_blackjack.DealerKarty) < 17)
			{
				_blackjack.DealerKarty.Add(_blackjack.VytiahniKartu(_blackjack.PotKarty));
				ZobrazKarty(_blackjack.DealerKarty, DealerCardsPanel);
			}
			foreach (var ruka in _blackjack.HracKarty)
			{
				int playerSum = _blackjack.SumaKariet(ruka);
				int dealerSum = _blackjack.SumaKariet(_blackjack.DealerKarty);

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
			}
			BalanceText.Text = $"Balance: {_blackjack.HracBalance}";
			UkonciHru();

		}

		private void UkonciHru()
		{
			_blackjack.Reset();

			PlayerFirstHandPanel.Children.Clear();
			DealerCardsPanel.Children.Clear();
			PlayerSecondHandPanel.Children.Clear();

			PlayerFirstSum.Text = string.Empty;
			DealerSum.Text = string.Empty;
			PlayerSecondSum.Text = string.Empty;
			PlayerSecondSum.Visibility = Visibility.Collapsed;

		}

		private void SplitButton_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Split button clicked.");

			if (_blackjack.JeParPrvejRuky())
			{
				Debug.WriteLine("Pair found, performing split.");

				SplitHand();
				UpdateHandLayoutForSplit();
				//ZobrazKartyPreRuky();
				ZobrazKarty(_blackjack.HracKarty[0], PlayerFirstHandPanel);
				ZobrazKarty(_blackjack.HracKarty[1], PlayerSecondHandPanel);

				AktualizujSplitTlačidlo();
			}
		}

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

		private void UpdateHandLayoutForSplit()
		{
			PlayerFirstHandPanel.HorizontalAlignment = HorizontalAlignment.Left;
			PlayerSecondHandPanel.Visibility = Visibility.Visible;
			SecondHandCall.Visibility = Visibility.Visible;
			PlayerSecondHandPanel.HorizontalAlignment = HorizontalAlignment.Right;
			PlayerSecondSum.Visibility = Visibility.Visible;
		}

		private void AktualizujSplitTlačidlo()
		{
			SplitButton.Visibility = _blackjack.JeParPrvejRuky() ? Visibility.Visible : Visibility.Collapsed;

		}

	}
}
