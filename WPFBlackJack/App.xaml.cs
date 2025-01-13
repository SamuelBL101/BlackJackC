using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WPFBlackJack
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static ServiceProvider _serviceProvider;

		public static ServiceProvider ServiceProvider => _serviceProvider;

		protected override void OnStartup(StartupEventArgs e)
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddHttpClient();
			_serviceProvider = serviceCollection.BuildServiceProvider();

			var mainWindow = new MainWindow();
			mainWindow.Show();
		}

	}

}
