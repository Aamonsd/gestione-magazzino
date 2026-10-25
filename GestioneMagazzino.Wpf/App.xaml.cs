using GestioneMagazzino.Repositories;
using GestioneMagazzino.Services;
using GestioneMagazzino.Wpf.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace GestioneMagazzino.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
     
    public partial class App : Application
    {
         
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IConfiguration configurazione = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            string stringaConnessione = configurazione.GetConnectionString("Magazzino")
            ?? throw new InvalidOperationException("Stringa di connessione 'Magazzino' mancante in appsettings.json");

            var servizio = new ServizioMagazzino(
                new SqlProdottiRepository(stringaConnessione),
                new SqlMovimentiRepository(stringaConnessione));

            var finestra = new MainWindow { DataContext = new MainViewModel(servizio) };
            finestra.Show();

        }



    }

}
