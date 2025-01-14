using System.Collections.Generic;
using System.Windows;
using LibShared;

namespace WPFBlackJack
{
	/// <summary>
	/// Okno zobrazujúce históriu hier používateľa.
	/// </summary>
	public partial class GameHistoryWindow : Window
	{

		/// <summary>
		/// Konštruktor triedy, ktorý inicializuje okno s históriou hier.
		/// </summary>
		/// <param name="gameHistory">Zoznam histórie hier, ktorý sa bude zobrazovať v okne.</param>
		public GameHistoryWindow(List<GameHistory> gameHistory)
		{
			InitializeComponent();
			GameHistoryListView.ItemsSource = gameHistory;
		}
	}
}