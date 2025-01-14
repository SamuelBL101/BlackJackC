using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Net.Http;
using WPFBlackJack.Service;

namespace WPFBlackJack
{
	public partial class App : Application
	{
		public static ServiceProvider ServiceProvider { get; private set; }

		public App()
		{
			var serviceCollection = new ServiceCollection();

			serviceCollection.AddSingleton<HttpClient>();

			serviceCollection.AddSingleton<MainWindow>();  
			serviceCollection.AddSingleton<WindowHra>();   
			serviceCollection.AddSingleton<GameHistoryApiClient>(); 

			ServiceProvider = serviceCollection.BuildServiceProvider();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
		}
	}
}