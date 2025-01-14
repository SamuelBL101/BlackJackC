using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace WPFBlackJack
{
	/// <summary>
	/// Trieda reprezentujúca kartu v hre Blackjack.
	/// </summary>
	public class Blackjack : INotifyPropertyChanged
	{
		private decimal _hracBalance;
		private int _aktualnaStavka;
		private int _idHraca;

		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// Získa alebo nastaví balance hráča.
		/// </summary>
		public int HracBalance
		{
			get => (int)_hracBalance;
			set => SetField(ref _hracBalance, value);
		}
		/// <summary>
		/// Získa alebo nastaví aktuálnu stávku v hre.
		/// </summary>
		public int AktualnaStavka
		{
			get => _aktualnaStavka;
			set => SetField(ref _aktualnaStavka, value);
		}
		/// <summary>
		/// Identifikátor hráča.
		/// </summary>
		public int IdHraca => _idHraca;

		/// <summary>
		/// Zoznam rúk hráča (každá ruka je zoznam kariet).
		/// </summary>
		public List<List<Karta>> HracKarty { get; set; }

		/// <summary>
		/// Zoznam kariet dealera.
		/// </summary>
		public List<Karta> DealerKarty { get; set; }

		/// <summary>
		/// Zoznam kariet, ktoré sú v balíčku.
		/// </summary>
		public List<Karta> PotKarty { get; set; }

		/// <summary>
		/// Index aktuálnej ruky hráča (v prípade viacerých rúk).
		/// </summary>
		public int AktualnaRuka { get; set; }


		private readonly Random _random = new();

		/// <summary>
		/// Konštruktor triedy Blackjack, inicializuje hráča a vytvára nový balíček kariet.
		/// </summary>
		public Blackjack(decimal Balance, int idhraca)
		{
			_idHraca = idhraca;
			HracKarty = new List<List<Karta>>();
			DealerKarty = new List<Karta>();
			PotKarty = new List<Karta>();
			HracBalance = (int)Balance; 
			PotKarty = VytvorKarty();
			AktualnaStavka = 0;
		}

		// <summary>
		/// Vytvorí nový balíček kariet pre hru.
		/// </summary>
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

		/// <summary>
		/// Vytiahne náhodnú kartu z balíčka.
		/// </summary>
		public Karta VytiahniKartu(List<Karta> karty)
		{
			if (karty.Count == 0)
			{
				karty = VytvorKarty();
			}

			int index = _random.Next(karty.Count);
			Karta vytiahnutaKarta = karty[index];
			karty.RemoveAt(index);

			return vytiahnutaKarta;
		}

		/// <summary>
		/// Vypočíta sumu hodnôt kariet v ruke (bez skrytých kariet).
		/// </summary>
		public int SumaKariet(List<Karta> karty)
		{
			int suma = 0;
			int pocetAsov = 0;
			foreach (var karta in karty.Where(k => !k.Skryta))
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

		/// <summary>
		/// Pridá kartu do ruka hráča.
		/// </summary>
		public void Hit(int rukaIndex)
		{
			HracKarty[rukaIndex].Add(VytiahniKartu(PotKarty));
		}

		/// <summary>
		/// Rozdá počiatočné karty hráčovi a dealerovi.
		/// </summary>
		public void Rozdanie(int Stavka)
		{
			AktualnaStavka = Stavka;
			HracBalance -= AktualnaStavka;
			;
			var karty = PotKarty;
			var prvaRuka = new List<Karta> { VytiahniKartu(karty), VytiahniKartu(karty) };
			HracKarty.Add(prvaRuka);  // Prvá ruka hráča

			DealerKarty.Add(VytiahniKartu(karty));
			DealerKarty.Add(VytiahniKartu(karty));
		}

		/// <summary>
		/// Rozdá karty pre druhú variantu rozdania (rovnaké hodnoty kariet).
		/// </summary>
		public void Rozdanie2(int Stavka)
		{
			AktualnaStavka = Stavka;
			HracBalance -= AktualnaStavka;

			var karty = PotKarty;

			Karta prvaKarta = VytiahniKartu(karty);
			Karta druhaKarta = VytiahniKartu(karty);

			while (prvaKarta.Hodnota != druhaKarta.Hodnota)
			{
				druhaKarta = VytiahniKartu(karty);
			}

			var prvaRuka = new List<Karta> { prvaKarta, druhaKarta };

			HracKarty.Add(prvaRuka);  

			DealerKarty.Add(VytiahniKartu(karty));
			DealerKarty.Add(VytiahniKartu(karty));
			DealerKarty[1].Skryta = true;
		}


		/// <summary>
		/// Skontroluje, či je prvá ruka hráča pár.
		/// </summary>
		public bool JeParPrvejRuky()
		{
			if (HracKarty.Count > 0)
			{
				var firstHand = HracKarty[0];
				if (firstHand.Count == 2)
				{
					var isPair = firstHand[0].Hodnota == firstHand[1].Hodnota;
					Debug.WriteLine($"Checking pair: {firstHand[0].Hodnota} vs {firstHand[1].Hodnota}, Result: {isPair}");
					return isPair;
				}
			}
			return false;
		}

		public void Reset()
		{
			HracKarty.Clear();
			DealerKarty.Clear();
		}
		/// <summary>
		/// Nastaví hodnotu vlastnosti pomocou setteru, pokiaľ sa hodnota zmenila.
		/// </summary>
		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		/// <summary>
		/// Vyvolá událosť PropertyChanged, ak je niektorá vlastnosť aktualizovaná.
		/// </summary>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
