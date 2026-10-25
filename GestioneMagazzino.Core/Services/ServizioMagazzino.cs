using GestioneMagazzino.Events;
using GestioneMagazzino.Models;
using GestioneMagazzino.Repositories;

namespace GestioneMagazzino.Services
{
    
    public class ServizioMagazzino
    {


        private readonly IProdottiRepository repositoryProdotti;
        private readonly IMovimentiRepository repositoryMovimenti;

        public event EventHandler<ScortaBassaEventArgs>? ScortaBassa;
        public event EventHandler<MovimentoRegistratoEventArgs>? MovimentoRegistrato;

        public ServizioMagazzino(IProdottiRepository repositoryProdotti, IMovimentiRepository repositoryMovimenti)
        {
            this.repositoryProdotti = repositoryProdotti;
            this.repositoryMovimenti = repositoryMovimenti;
        }  

        public Movimento RegistraMovimento(Prodotto prodotto, TipoMovimento tipo,int quantita)
        {
            if (quantita <= 0) throw new ArgumentException("La quantità deve essere maggiore di zero");
            //REGISTRA NEL DATABASE: AGGIORNA LA SCORTA E INSERISCE IL MOVIMENTO IN UNA TRANSAZIONE
            Movimento newMovimento =repositoryMovimenti.Registra(prodotto.CodiceProdotto, tipo, quantita);

            //AGGIORNA LA QUANTITA DELL'OGGETTO IN MEMORIA CON QUELLA DEL DATABASE
            Prodotto? aggiornato = repositoryProdotti.TrovaPerCodice(prodotto.CodiceProdotto);
            if(aggiornato!=null) prodotto.QuantitaAttuale = aggiornato.QuantitaAttuale;

            //SCATENA EVENTO MovimentoRegistrato
            MovimentoRegistrato?.Invoke(this, new MovimentoRegistratoEventArgs(newMovimento));

            if (prodotto.SottoSoglia())
            {
                ScortaBassa?.Invoke(this, new ScortaBassaEventArgs(prodotto, prodotto.QuantitaAttuale, prodotto.SogliaMinima));
            }

            return newMovimento;
        }


        public void AggiungiProdotto(Prodotto prodotto)
        {
            List<Prodotto> listaProdotti = repositoryProdotti.OttieniTutti();

            if (repositoryProdotti.TrovaPerCodice(prodotto.CodiceProdotto) != null) 

            if (prodotto.PrezzoUnitario <= 0) throw new ArgumentException("Prezzo non puo essere minore o uguale a 0");

            if (prodotto.SogliaMinima < 0) throw new ArgumentException("La soglia non puo essere minore di 0");

            repositoryProdotti.Aggiungi(prodotto);
             
        }

        public List<Prodotto> OttieniProdotti()
        {
            List<Prodotto> tuttiProdotti = repositoryProdotti.OttieniTutti();
            return tuttiProdotti;
        }


        public Prodotto TrovaProdotto(string codice)
        {
            Prodotto? prodottoTrovato = repositoryProdotti.TrovaPerCodice(codice);
            if (prodottoTrovato is null) throw new ArgumentException("prodotto non trovato");
            return prodottoTrovato;
             
        }

        public List<Movimento> OttieniMovimenti()
        {
            List<Movimento> tuttiMovimenti = repositoryMovimenti.OttieniTutti();
            return tuttiMovimenti;
        }

         


    }
}