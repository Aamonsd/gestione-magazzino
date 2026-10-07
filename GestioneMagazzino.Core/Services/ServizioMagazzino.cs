using GestioneMagazzino.Events;
using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Models;
using GestioneMagazzino.Repositories;



namespace GestioneMagazzino.Services
{
    
    public class ServizioMagazzino
    {
        private static readonly string CartellaDati = Path.Combine(Environment
            .GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GestioneMagazzino");

        private static readonly string PercorsoProdotti = Path.Combine(CartellaDati,"prodotti.json");
        private static readonly string PercorsoMovimenti = Path.Combine(CartellaDati, "movimenti.json");

        private Repository<Prodotto> repositoryProdotti = new Repository<Prodotto>();
        private Repository<Movimento> repositoryMovimenti = new Repository<Movimento>();

        private int contatoreMovimenti = 0;

        public event EventHandler<ScortaBassaEventArgs>? ScortaBassa;
        public event EventHandler<MovimentoRegistratoEventArgs>? MovimentoRegistrato;

        public ServizioMagazzino()
        {
            Directory.CreateDirectory(CartellaDati);
        }

        public Movimento RegistraMovimento(Prodotto prodotto, TipoMovimento tipo,int quantita)
        {
            if (quantita <= 0) throw new ArgumentException("La quantità deve essere maggiore di zero");

            if (tipo == TipoMovimento.Scarico)
            {
                if (prodotto.QuantitaAttuale < quantita)
                {
                    throw new ScortaInsufficienteException($"Scorta insufficiente per {prodotto.Nome}: disponibili {prodotto.QuantitaAttuale},richiesti {quantita}");
                }
            }

            //AGGIORNA QUANTITA ATTUALE;
            if (tipo == TipoMovimento.Carico) prodotto.QuantitaAttuale += quantita;
            else if (tipo == TipoMovimento.Scarico) prodotto.QuantitaAttuale -= quantita;

            //GENERA ID UNIVICO PER IL MOVIMENTO
            contatoreMovimenti++;
            string id = "MOV-" + contatoreMovimenti;

            //CREA UN NUOVO MOVIMENTO
            Movimento newMovimento = new Movimento(id, prodotto.CodiceProdotto, tipo, quantita);

            //MOVIMENTO AGGIUNTO ALLA REPOSITORY
            repositoryMovimenti.Aggiungi(newMovimento);
            SalvaDati();

            //SCATENA EVENTO MovimentoRegistrato
            MovimentoRegistrato?.Invoke(this,new MovimentoRegistratoEventArgs(newMovimento));

            //CONTROLLA SE IL PRODOTTO E SOTTO SOGLIA
            if (prodotto.SottoSoglia())
            {
                ScortaBassa?.Invoke(this, new ScortaBassaEventArgs(prodotto, prodotto.QuantitaAttuale, prodotto.SogliaMinima));
            }

            return newMovimento;
            
        }


        public void AggiungiProdotto(Prodotto prodotto)
        {
            List<Prodotto> listaProdotti = repositoryProdotti.OttieniTutti();

            if (listaProdotti.Any(p => string.Equals(p.CodiceProdotto, prodotto.CodiceProdotto, StringComparison.OrdinalIgnoreCase)))
                throw new ProdottoGiaEsistenteException($"Esiste già un prodotto con codice {prodotto.CodiceProdotto}");

            if (prodotto.PrezzoUnitario <= 0) throw new ArgumentException("Prezzo non puo essere minore o uguale a 0");

            if (prodotto.SogliaMinima < 0) throw new ArgumentException("La soglia non puo essere minore di 0");

            repositoryProdotti.Aggiungi(prodotto);
            SalvaDati();
        }

        public List<Prodotto> OttieniProdotti()
        {
            List<Prodotto> tuttiProdotti = repositoryProdotti.OttieniTutti();
            return tuttiProdotti;
        }


        public Prodotto TrovaProdotto(string codice)
        {
            Prodotto prodottoTrovato = repositoryProdotti.TrovaPerIdentificatore(codice);
            return prodottoTrovato;
             
        }

        public List<Movimento> OttieniMovimenti()
        {
            List<Movimento> tuttiMovimenti = repositoryMovimenti.OttieniTutti();
            return tuttiMovimenti;
        }

        public void SalvaDati()
        {
            repositoryProdotti.SalvaFile(PercorsoProdotti);
            repositoryMovimenti.SalvaFile(PercorsoMovimenti);
        }
        public void CaricaDati()
        {
            repositoryProdotti.CaricaFile(PercorsoProdotti);
            repositoryMovimenti.CaricaFile(PercorsoMovimenti);
            contatoreMovimenti = 0;
            foreach (var item in repositoryMovimenti.OttieniTutti())
            {
                int numero = int.Parse(item.Identificatore.Replace("MOV-", ""));
                if (numero > contatoreMovimenti) contatoreMovimenti = numero;
            }
        }


    }
}