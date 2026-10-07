using GestioneMagazzino.Services;
using GestioneMagazzino.Wpf.Commands;
using System.Windows.Input;

namespace GestioneMagazzino.Wpf.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        private readonly ServizioMagazzino servizio;

        public ICommand MostraDashboardCommand { get; }
        public ICommand MostraProdottiCommand { get; }
        public ICommand MostraRegistraMovimentoCommand { get; }

        public ICommand MostraStoricoMovimentiCommand { get; }

        private object? viewModelCorrente;

        public object? ViewModelCorrente
        {
            get { return viewModelCorrente; }
            private set
            {
                if (viewModelCorrente is IDisposable vecchio) vecchio.Dispose();
                viewModelCorrente = value;
                OnPropertyChanged(nameof(ViewModelCorrente));
            }
        }

        public MainViewModel(ServizioMagazzino servizio)
        {
            this.servizio = servizio;
            ApriDashboard();
            MostraDashboardCommand = new RelayCommand(ApriDashboard);
            MostraProdottiCommand = new RelayCommand(ApriProdotti);
            MostraRegistraMovimentoCommand = new RelayCommand(ApriRegistraMovimento);
            MostraStoricoMovimentiCommand = new RelayCommand(ApriStoricoMovimenti);
            
             
        }

        
        private void ApriDashboard()
        {
            ViewModelCorrente = new DashboardViewModel(servizio);
        }
        private void ApriNuovoProdotto()
        {
            ViewModelCorrente = new NuovoProdottoViewModel(servizio, ApriProdotti);
        }
        private void ApriProdotti()
        {
            ViewModelCorrente = new ProdottiViewModel(servizio, ApriNuovoProdotto);
        }

        private void ApriRegistraMovimento()
        {
            ViewModelCorrente = new RegistraMovimentoViewModel(servizio);
        }

        private void ApriStoricoMovimenti()
        {
            ViewModelCorrente = new StoricoMovimentiViewModel(servizio);
        }

     
    }
}
