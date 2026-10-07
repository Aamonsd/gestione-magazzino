using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Models;
using GestioneMagazzino.Services;
using GestioneMagazzino.Wpf.Commands;
using System.Windows.Input;

namespace GestioneMagazzino.Wpf.ViewModels
{
    public class NuovoProdottoViewModel:ViewModelBase
    {
        private readonly ServizioMagazzino servizio;
        private readonly Action tornaAProdotti;

        private string messaggioErrore="";
        public string MessaggioErrore
        {
            get => messaggioErrore;
            set
            {
                if (messaggioErrore == value)
                    return;

                messaggioErrore = value;
                OnPropertyChanged(nameof(MessaggioErrore));
            }
        }

        public ICommand SalvaCommand { get; }
        public string Codice { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Prezzo { get; set; } = "";
        public string SogliaMinima { get; set; } = "";


        public NuovoProdottoViewModel(ServizioMagazzino servizio, Action tornaAProdotti)
        {
            this.servizio = servizio;
            this.tornaAProdotti = tornaAProdotti;
            SalvaCommand = new RelayCommand(Salva);
        }

        private void Salva()
        {

            if (string.IsNullOrWhiteSpace(Codice))
            {
                MessaggioErrore = "inserisci codice";
                return;
            }

            if (string.IsNullOrWhiteSpace(Nome))
            {
                MessaggioErrore = "inserisci nome prodotto";
                return;
            }

            if (!decimal.TryParse(Prezzo,out decimal prezzo))
            {
                MessaggioErrore = "il prezzo deve essere un numero valido esempio 2,50";
                return;
            }

            if(!int.TryParse(SogliaMinima,out int sogliaMinima))
            {
                MessaggioErrore = "la soglia minima deve essere un numero intero, ad esempio 10";
                return;
            }

             

            Prodotto nuovoProdotto = new Prodotto(Codice, Nome, prezzo, sogliaMinima);
            try
            {
                servizio.AggiungiProdotto(nuovoProdotto);
                tornaAProdotti();
            }
            catch (ProdottoGiaEsistenteException ex)
            {
                MessaggioErrore = ex.Message;
            }
            catch(ArgumentException ex)
            {
                MessaggioErrore = ex.Message;
            }
            

        }
    }
}
