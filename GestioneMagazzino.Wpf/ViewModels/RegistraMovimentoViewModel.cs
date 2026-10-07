using GestioneMagazzino.Events;
using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Models;
using GestioneMagazzino.Services;
using GestioneMagazzino.Wpf.Commands;
using System.Windows.Input;

namespace GestioneMagazzino.Wpf.ViewModels
{
    public class RegistraMovimentoViewModel:ViewModelBase,IDisposable
    {

        

        private readonly ServizioMagazzino servizio;
        public ICommand RegistraCommand { get; }
        public ICommand SelezionaCaricoCommand { get; }
        public ICommand SelezionaScaricoCommand { get; }

        private TipoMovimento tipoSelezionato = TipoMovimento.Carico;

        private string messaggioConferma="";

        public string MessaggioConferma
        {
            get { return messaggioConferma; }
            set
            {
                messaggioConferma = value;
                OnPropertyChanged(nameof(MessaggioConferma));
            }
             
        }

        private string messaggioAvviso="";

        public string MessaggioAvviso
        {
            get { return messaggioAvviso; }
            set
            {
                messaggioAvviso = value;
                OnPropertyChanged(nameof(MessaggioAvviso));
            }
        }

        private string messaggioErrore = "";
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


        private string quantita="";

        public string Quantita
        {
            get { return quantita; }
            set
            {
                
                quantita = value;
                OnPropertyChanged(nameof(quantita));
            }
        }




        public decimal? ValoreScorta => prodottoSelezionato?.ValoreScorta();

        public List<Prodotto> ListaProdotti => servizio.OttieniProdotti()
            .OrderBy(p => p.CodiceProdotto)
            .ToList();

        private Prodotto? prodottoSelezionato;

        public Prodotto? ProdottoSelezionato
        {
            get { return prodottoSelezionato; }
            set 
            { 
                prodottoSelezionato = value;
                OnPropertyChanged(nameof(ProdottoSelezionato));
                OnPropertyChanged(nameof(ValoreScorta));
            }
        }

    
        private void OnMovimentoRegistrato(object? sender, MovimentoRegistratoEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Handler eseguito");
            MessaggioConferma = $"Movimento {e.Movimento.Identificatore} registrato";
        }
        private void OnScortaBassa(object? sender,ScortaBassaEventArgs e)
        {
            MessaggioAvviso = $"Attenzione: {e.Prodotto.Nome} e sotto soglia {e.QuantitaAttuale}/{e.Prodotto.SogliaMinima}";
        }
        public RegistraMovimentoViewModel(ServizioMagazzino servizio)
        {
            this.servizio = servizio;
            servizio.MovimentoRegistrato += OnMovimentoRegistrato;
            servizio.ScortaBassa += OnScortaBassa;

            SelezionaCaricoCommand = new RelayCommand(ImpostaCarico);
            SelezionaScaricoCommand = new RelayCommand(ImpostaScarico);
            RegistraCommand = new RelayCommand(Registra);
            
        }

        private void ImpostaCarico()
        {
            tipoSelezionato = TipoMovimento.Carico;
        }

        private void ImpostaScarico()
        {
            tipoSelezionato = TipoMovimento.Scarico;
        }

        public void Registra()
        {
            MessaggioErrore = "";
            MessaggioAvviso = "";
            MessaggioConferma = "";

            if (ProdottoSelezionato is null)
            {
                MessaggioErrore = "Seleziona prodotto";
                return;
            }

            if (string.IsNullOrWhiteSpace(Quantita))
            {
                MessaggioErrore = "Inserisci quantità";
                return;
            }
            if (!int.TryParse(Quantita,out int quantita))
            {
                MessaggioErrore = "La quantità deve essere un numero intero";
                return;
            }

            try
            {
                servizio.RegistraMovimento(ProdottoSelezionato, tipoSelezionato, quantita);
                OnPropertyChanged(nameof(ValoreScorta));
                OnPropertyChanged(nameof(ProdottoSelezionato));
                Quantita = "";
            }
            catch (ScortaInsufficienteException ex)
            {
                MessaggioErrore = ex.Message;
            }
            catch (ArgumentException ex)
            {
                MessaggioErrore = ex.Message;

            }
        }

        public void Dispose()
        {
            servizio.MovimentoRegistrato -= OnMovimentoRegistrato;
            servizio.ScortaBassa -= OnScortaBassa;
        }
    }
}
