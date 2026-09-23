
using System;
using GestioneMagazzino.Models;
using GestioneMagazzino.Events;
using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Repositories;
using System.Net.Http.Headers;

namespace GestioneMagazzino.Services
{
    public class ServizioMagazzino
    {
        private const string PercorsoProdotti = "prodotti.json";
        private const string PercorsoMovimenti = "movimenti.json";
        private Repository<Prodotto> repositoryProdotti = new Repository<Prodotto>();
        private Repository<Movimento> repositoryMovimenti = new Repository<Movimento>();

        private int ContatoreMovimenti = 0;

        public event EventHandler<ScortaBassaEventArgs> ScortaBassa;
        public event EventHandler<MovimentoRegistratoEventArgs> MovimentoRegistrato;

        public Movimento RegistraMovimento(Prodotto prodotto, TipoMovimento tipo,int quantita)
        {
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
            ContatoreMovimenti++;
            string ID = "MOV-" + ContatoreMovimenti;

            //CREA UN NUOVO MOVIMENTO
            Movimento newMovimento = new Movimento(ID, prodotto.CodiceProdotto, tipo, quantita);

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
            ContatoreMovimenti = 0;
            foreach (var item in repositoryMovimenti.OttieniTutti())
            {
                int numero = int.Parse(item.Identificatore.Replace("MOV-", ""));
                if (numero > ContatoreMovimenti) ContatoreMovimenti = numero;
            }
        }


    }
}