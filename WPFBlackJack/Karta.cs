using System;

namespace WPFBlackJack
{
	/// <summary>
	/// Trieda reprezentujúca kartu v hre Blackjack.
	/// Obsahuje vlastnosti pre hodnotu, symbol a farbu karty,
	/// ako aj cestu k obrázku a stav skrytosti karty.
	/// </summary>
	public class Karta
	{
		/// <summary>
		/// Hodnota karty (napr. 2, 3, 10, J, Q, K, A).
		/// </summary>
		public int Hodnota { get; set; }

		/// <summary>
		/// Symbol karty (napr. "hearts", "diamonds", "clubs", "spades").
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		/// Farba karty (napr. "red", "black").
		/// </summary>
		public string Farba { get; set; }

		/// <summary>
		/// Vytvorí cestu k obrázku karty, ktorá sa nachádza v adresári s obrázkami.
		/// </summary>
		public string Obrazok => $@"C:\Users\billy\source\repos\UserData\WPFBlackJack\Images\{Farba}_of_{Symbol}.png";

		/// <summary>
		/// Určuje, či je karta skrytá (napr. pre kartu dealera, ktorá je zakrytá).
		/// </summary>
		public bool Skryta { get; set; }

		/// <summary>
		/// Konštruktor na inicializáciu karty s hodnotou, symbolom a farbou.
		/// Predvolená hodnota pre Skryta je false (karta nie je skrytá).
		/// </summary>
		/// <param name="hodnota">Hodnota karty (napr. 2, 3, ... A).</param>
		/// <param name="symbol">Symbol karty (napr. "hearts", "diamonds").</param>
		/// <param name="farba">Farba karty (napr. "red", "black").</param>
		public Karta(int hodnota, string symbol, string farba)
		{
			Hodnota = hodnota;
			Symbol = symbol;
			Farba = farba;
			Skryta = false;
		}
	}
}