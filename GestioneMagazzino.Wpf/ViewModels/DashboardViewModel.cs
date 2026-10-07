using GestioneMagazzino.Models;
using GestioneMagazzino.Services;


namespace GestioneMagazzino.Wpf.ViewModels
{
    public class DashboardViewModel
    {
        private readonly ServizioMagazzino servizio;
        public int ProdottiTotali => servizio.OttieniProdotti().Count;
        public int SottoSoglia => servizio.OttieniProdotti().Count(p => p.SottoSoglia());
        public decimal ValoreMagazzino => servizio.OttieniProdotti().Sum(p=>p.ValoreScorta());
        public int MovimentiOggi => servizio.OttieniMovimenti().Count(m=>m.DataMovimento.Date==DateTime.Today);
        public List<Prodotto> ProdottiSottoSoglia => servizio.OttieniProdotti().Where(p => p.SottoSoglia()).ToList();
        public List<MovimentoVisualizzato> UltimiMovimenti => servizio.OttieniMovimenti()
            .OrderByDescending(m => m.DataMovimento)
            .Take(5)
            .Select(m => new MovimentoVisualizzato(servizio.TrovaProdotto(m.CodiceProdotto).Nome,m.Tipo,m.Quantita)).ToList();

        public DashboardViewModel(ServizioMagazzino servizio)
        {
            this.servizio = servizio;
        }
    }
}
