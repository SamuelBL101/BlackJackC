using System;

namespace WPFBlackJack
{
	public class Karta
	{
		public int Hodnota { get; set; } 
		public string Symbol { get; set; }
		public string Farba { get; set; } 

		public string Obrazok
		{
			get
			{
				return $@"C:\Users\billy\source\repos\UserData\WPFBlackJack\Images\{Farba}_of_{Symbol}.png";
				return $"pack://application:,,,/Images/{Farba}_of_{Symbol}.png";

				return $"/Images/{Farba}_of_{Symbol}.png";
			}
		}

		public Karta(int hodnota, string symbol, string farba)
		{
			Hodnota = hodnota;
			Symbol = symbol;
			Farba = farba;
		}
	}
}