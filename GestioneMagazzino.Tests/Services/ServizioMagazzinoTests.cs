using GestioneMagazzino.Events;
using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Models;
using GestioneMagazzino.Services;
using GestioneMagazzino.Tests.Fakes;
using Xunit;

namespace GestioneMagazzino.Tests.Services
{
    public class ServizioMagazzinoTests
    {

        private readonly FakeProdottiRepository prodotti;
        private readonly FakeMovimentiRepository movimenti;
        private readonly ServizioMagazzino servizio;

        public ServizioMagazzinoTests()
        {
            prodotti = new FakeProdottiRepository();
            movimenti = new FakeMovimentiRepository(prodotti);
            servizio = new ServizioMagazzino(prodotti,movimenti);
        }

        private Prodotto CreaProdotto(int soglia = 5, int scortaIniziale = 0)
        {
            var prodotto = new Prodotto("P1", "Vite", 0.10m, soglia);
            servizio.AggiungiProdotto(prodotto);
            if (scortaIniziale > 0)
                servizio.RegistraMovimento(prodotto, TipoMovimento.Carico, scortaIniziale);
            return prodotto;
        }

        // ---------- AggiungiProdotto ----------

        [Fact]

        public void AggiungiProdotto_CodiceNuovo_LoSalva()
        {

            var prodotto = new Prodotto("P1", "Vite", 0.10m, 5);
            servizio.AggiungiProdotto(prodotto);
            Assert.NotNull(prodotti.TrovaPerCodice("P1"));

        }

        [Fact]
        public void AggiungiProdotto_CodiceDuplicato_LanciaProdottoGiaEsistente()
        {
            // Arrange
            CreaProdotto();
            var doppione = new Prodotto("p1", "Altro", 1m, 0);   // stesso codice, in minuscolo

            // Act + Assert
            Assert.Throws<ProdottoGiaEsistenteException>(() => servizio.AggiungiProdotto(doppione));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AggiungiProdotto_PrezzoNonPositivo_LanciaArgumentException(int prezzo)
        {
            // Arrange
            var prodotto = new Prodotto("P1", "Vite", prezzo, 5);

            // Act + Assert
            Assert.Throws<ArgumentException>(() => servizio.AggiungiProdotto(prodotto));
            Assert.Null(prodotti.TrovaPerCodice("P1"));   // e non è stato salvato
        }

        [Fact]
        public void AggiungiProdotto_SogliaNegativa_LanciaArgumentException()
        {
            // Arrange
            var prodotto = new Prodotto("P1", "Vite", 0.10m, -1);

            // Act + Assert
            Assert.Throws<ArgumentException>(() => servizio.AggiungiProdotto(prodotto));
        }

        // ---------- RegistraMovimento ----------

        [Fact]
        public void RegistraMovimento_Carico_AumentaLaQuantita()
        {
            // Arrange
            var prodotto = CreaProdotto();

            // Act
            servizio.RegistraMovimento(prodotto, TipoMovimento.Carico, 10);

            // Assert
            Assert.Equal(10, prodotto.QuantitaAttuale);
        }

        [Fact]
        public void RegistraMovimento_Scarico_DiminuisceLaQuantita()
        {
            // Arrange
            var prodotto = CreaProdotto(scortaIniziale: 10);

            // Act
            servizio.RegistraMovimento(prodotto, TipoMovimento.Scarico, 4);

            // Assert
            Assert.Equal(6, prodotto.QuantitaAttuale);
        }
        [Fact]
        public void RegistraMovimento_ScaricoOltreLaScorta_LanciaENonCambiaNiente()
        {
            // Arrange
            var prodotto = CreaProdotto(scortaIniziale: 3);

            // Act + Assert
            Assert.Throws<ScortaInsufficienteException>(
                () => servizio.RegistraMovimento(prodotto, TipoMovimento.Scarico, 5));
            Assert.Equal(3, prodotto.QuantitaAttuale);          // scorta invariata
            Assert.Single(servizio.OttieniMovimenti());          // solo il carico iniziale
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void RegistraMovimento_QuantitaNonPositiva_LanciaArgumentException(int quantita)
        {
            // Arrange
            var prodotto = CreaProdotto();

            // Act + Assert
            Assert.Throws<ArgumentException>(
                () => servizio.RegistraMovimento(prodotto, TipoMovimento.Carico, quantita));
        }

        [Fact]
        public void RegistraMovimento_RestituisceIlMovimentoConIdEDati()
        {
            // Arrange
            var prodotto = CreaProdotto();

            // Act
            Movimento movimento = servizio.RegistraMovimento(prodotto, TipoMovimento.Carico, 7);

            // Assert
            Assert.Equal("MOV-1", movimento.Identificatore);
            Assert.Equal("P1", movimento.CodiceProdotto);
            Assert.Equal(TipoMovimento.Carico, movimento.Tipo);
            Assert.Equal(7, movimento.Quantita);
        }

        // ---------- Eventi ----------

        [Fact]
        public void RegistraMovimento_LanciaLEventoMovimentoRegistrato()
        {
            // Arrange
            var prodotto = CreaProdotto();
            MovimentoRegistratoEventArgs? ricevuto = null;
            servizio.MovimentoRegistrato += (sender, e) => ricevuto = e;

            // Act
            servizio.RegistraMovimento(prodotto, TipoMovimento.Carico, 10);

            // Assert
            Assert.NotNull(ricevuto);
            Assert.Equal(10, ricevuto.Movimento.Quantita);
        }

        [Fact]
        public void RegistraMovimento_SottoSoglia_LanciaScortaBassa()
        {
            // Arrange
            var prodotto = CreaProdotto(soglia: 5, scortaIniziale: 10);
            bool scortaBassaLanciato = false;
            servizio.ScortaBassa += (sender, e) => scortaBassaLanciato = true;

            // Act: 10 - 6 = 4, sotto la soglia di 5
            servizio.RegistraMovimento(prodotto, TipoMovimento.Scarico, 6);

            // Assert
            Assert.True(scortaBassaLanciato);
        }

        [Fact]
        public void RegistraMovimento_SopraSoglia_NonLanciaScortaBassa()
        {
            // Arrange
            var prodotto = CreaProdotto(soglia: 5, scortaIniziale: 10);
            bool scortaBassaLanciato = false;
            servizio.ScortaBassa += (sender, e) => scortaBassaLanciato = true;

            // Act: 10 - 2 = 8, sopra la soglia
            servizio.RegistraMovimento(prodotto, TipoMovimento.Scarico, 2);

            // Assert
            Assert.False(scortaBassaLanciato);
        }

        // ---------- TrovaProdotto ----------

        [Fact]
        public void TrovaProdotto_CodiceInesistente_LanciaArgumentException()
        {
            Assert.Throws<ArgumentException>(() => servizio.TrovaProdotto("NONESISTE"));
        }

    }

}
