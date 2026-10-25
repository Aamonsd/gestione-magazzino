using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Models;
using GestioneMagazzino.Repositories;
 

namespace GestioneMagazzino.Tests.Fakes
{
    public class FakeMovimentiRepository: IMovimentiRepository
    {
        private readonly List<Movimento> movimenti = new();
        private readonly FakeProdottiRepository prodotti;
        private int prossimoId = 1;   // fa le veci di IDENTITY

        public FakeMovimentiRepository(FakeProdottiRepository prodotti)
        {
            this.prodotti = prodotti;
        }

        public List<Movimento> OttieniTutti()
        {
            return new List<Movimento>(movimenti);
        }

        public Movimento Registra(string codiceProdotto,TipoMovimento tipo, int quantita)
        {
            int variazione = tipo == TipoMovimento.Carico ? quantita : -quantita;

            // stessa regola della WHERE nell'UPDATE: prodotto esistente e scorta mai negativa
            Prodotto? prodotto = prodotti.TrovaPerCodice(codiceProdotto);
            if (prodotto == null || prodotto.QuantitaAttuale + variazione < 0)
                throw new ScortaInsufficienteException($"Scorta insufficiente per {codiceProdotto}");

            prodotto.QuantitaAttuale += variazione;

            var movimento = new Movimento(prossimoId++, codiceProdotto, tipo, quantita, DateTime.Now);
            movimenti.Add(movimento);
            return movimento;


        }

    }
}
