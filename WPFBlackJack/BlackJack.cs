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

		public List<List<Karta>> HracKarty { get; set; }
		public List<Karta> DealerKarty { get; set; }
		public List<Karta> PotKarty { get; set; }

		private readonly Random _random = new();

		public Blackjack()
		{
			HracKarty = new List<List<Karta>>();
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

		public void Hit(int rukaIndex = 0)
		{
			HracKarty[rukaIndex].Add(VytiahniKartu(PotKarty));
		}

		public void Rozdanie(int Stavka)
		{
			AktualnaStavka = Stavka;
			HracBalance -= AktualnaStavka;

			var karty = PotKarty;
			var prvaRuka = new List<Karta> { VytiahniKartu(karty), VytiahniKartu(karty) };
			HracKarty.Add(prvaRuka);  // Prvá ruka hráča

			DealerKarty.Add(VytiahniKartu(karty));
			DealerKarty.Add(VytiahniKartu(karty));
		}

		public bool JePar(List<Karta> karty)
		{
			return karty.Count == 2 && karty[0].Hodnota == karty[1].Hodnota;
		}
		public bool JeParPrvejRuky()
		{
			if (HracKarty.Count > 0 && HracKarty[0].Count == 2)
			{
				return HracKarty[0][0].Hodnota == HracKarty[0][1].Hodnota;
			}
			return false;
		}


		public void Split()
		{
			if (HracKarty.Count == 1 && HracKarty[0].Count == 2 && HracKarty[0][0].Hodnota == HracKarty[0][1].Hodnota)
			{
				var prvaRuka = new List<Karta> { HracKarty[0][0], VytiahniKartu(PotKarty) };
				var druhaRuka = new List<Karta> { HracKarty[0][1], VytiahniKartu(PotKarty) };

				HracKarty.Clear();
				HracKarty.Add(prvaRuka);
				HracKarty.Add(druhaRuka);
			}
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
