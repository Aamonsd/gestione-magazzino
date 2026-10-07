using GestioneMagazzino.Services;

namespace GestioneMagazzino.Wpf.ViewModels
{
    public class StoricoMovimentiViewModel: ViewModelBase
    {
        private readonly ServizioMagazzino servizio;

        public List<MovimentoStorico> ListaMovimenti => servizio.OttieniMovimenti()
            .OrderByDescending(m => m.DataMovimento)
            .Select(m => new MovimentoStorico(m.DataMovimento, m.Identificatore, servizio.TrovaProdotto(m.CodiceProdotto).Nome, m.Tipo, m.Quantita))
            .ToList();

        public StoricoMovimentiViewModel(ServizioMagazzino servizio)
        {
            this.servizio = servizio;
        }
    }
}
