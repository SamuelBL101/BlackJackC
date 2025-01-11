using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPFBlackJack
{
	public class Blackjack : INotifyPropertyChanged
	{
		private int _hracBalance;
		private int _aktualnaStavka;

		public event PropertyChangedEventHandler PropertyChanged;
		public int HracBalance
		{
			get => _hracBalance;
			set => SetField(ref _hracBalance, value);
		}
		public int AktualnaStavka
		{
			get => _aktualnaStavka;
			set => SetField(ref _aktualnaStavka, value);
		}

		public List<Karta> HracKarty { get; set; }
		public List<Karta> DealerKarty { get; set; }
		public List<Karta> PotKarty { get; set; }

		private readonly Random _random = new();

		public Blackjack()
		{
			HracKarty = new List<Karta>();
			DealerKarty = new List<Karta>();
			PotKarty = new List<Karta>();
			HracBalance = 1000; 
			PotKarty = VytvorKarty();
			AktualnaStavka = 0;
		}

		public List<Karta> VytvorKarty()
		{
			List<Karta> karty = new List<Karta>();

			string[] farby = { "hearts", "diamonds", "clubs", "spades" };
			string[] hodnoty = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king", "ace" };
			foreach (var farba in farby)
			{
				foreach (var hodnota in hodnoty)
				{
					int kartaHodnota = (hodnota == "ace") ? 11 : (hodnota == "king" || hodnota == "queen" || hodnota == "jack") ? 10 : int.Parse(hodnota);
					karty.Add(new Karta(kartaHodnota, farba, hodnota));
				}
			}

			return karty;
		}

		public Karta VytiahniKartu(List<Karta> karty)
		{
			int index = _random.Next(karty.Count);
			Karta vytiahnutaKarta = karty[index];
			karty.RemoveAt(index);
			return vytiahnutaKarta;
		}

		public int SumaKariet(List<Karta> karty)
		{
			int suma = 0;
			int pocetAsov = 0;
			foreach (var karta in karty)
			{
				suma += karta.Hodnota;
				if (karta.Hodnota == 11) pocetAsov++;
			}

			while (suma > 21 && pocetAsov > 0)
			{
				suma -= 10;
				pocetAsov--;
			}

			return suma;
		}

		public void Hit()
		{
			HracKarty.Add(VytiahniKartu(PotKarty));
		}

		public void Rozdanie(int Stavka)
		{
			AktualnaStavka = Stavka;
			HracBalance -= AktualnaStavka;

			var karty = PotKarty;
			HracKarty.Add(VytiahniKartu(karty));
			DealerKarty.Add(VytiahniKartu(karty));
			HracKarty.Add(VytiahniKartu(karty));
			DealerKarty.Add(VytiahniKartu(karty));
		}

		public void Reset()
		{
			HracKarty.Clear();
			DealerKarty.Clear();
		}
		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
