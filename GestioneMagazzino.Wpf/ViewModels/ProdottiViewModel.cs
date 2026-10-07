using GestioneMagazzino.Services;
using GestioneMagazzino.Wpf.Commands;
using System.Windows.Input;

namespace GestioneMagazzino.Wpf.ViewModels
{
    public class ProdottiViewModel:ViewModelBase
    {

        private readonly ServizioMagazzino servizio;
        public ICommand AggiungiProdottoCommand { get; }

        public List<ProdottoVisualizzato> ListaProdotti => servizio.OttieniProdotti()
            .OrderBy(p => p.CodiceProdotto)
            .Select(p => new ProdottoVisualizzato(p.CodiceProdotto, p.Nome, p.PrezzoUnitario, p.QuantitaAttuale,p.SogliaMinima, p.SottoSoglia()))
            .ToList();

        public ProdottiViewModel(ServizioMagazzino servizio,Action apriNuovoProdotto)
        {
            this.servizio = servizio;
            AggiungiProdottoCommand = new RelayCommand(apriNuovoProdotto);
        }
        
    }
}
