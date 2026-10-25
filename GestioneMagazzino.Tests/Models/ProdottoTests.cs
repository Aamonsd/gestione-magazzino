using GestioneMagazzino.Models;
using Xunit;

namespace GestioneMagazzino.Tests.Models
{
    public class ProdottoTests
    {
        [Fact]
        public void NuovoProdotto_HaQuantitaZero()
        {
            // Arrange
            var prodotto = new Prodotto("P1", "Vite", 0.10m, 50);

            // Act
            int quantita = prodotto.QuantitaAttuale;

            // Assert
            Assert.Equal(0, quantita);
        }

        [Fact]
        public void ValoreScorta_RestituisceQuantitaPerPrezzo()
        {
            var prodotto = new Prodotto("P1", "Vite", 2.50m,0);
            prodotto.QuantitaAttuale = 4;

            decimal valore = prodotto.ValoreScorta();

            Assert.Equal(10.00m, valore);

        }

        [Theory]
        [InlineData(4,5,true)]
        [InlineData(5, 5, false)]
        [InlineData(6, 5, false)]
        public void SottoSoglia_ConfrontaQuantitaESoglia(int quantita,int soglia,bool atteso)
        {
            var prodotto = new Prodotto("P1", "Vite", 2.10m, soglia);
            prodotto.QuantitaAttuale = quantita;

            bool risultato = prodotto.SottoSoglia();

            Assert.Equal(atteso, risultato);
        }


    }

    
}
