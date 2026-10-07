
using GestioneMagazzino.Models;
namespace GestioneMagazzino.Events

{
    public class ScortaBassaEventArgs : EventArgs
    {
        public Prodotto Prodotto { get; }
        public int QuantitaAttuale { get; }
        public int SogliaMinima { get; }
        public ScortaBassaEventArgs(Prodotto prodotto, int quantitaAttuale, int sogliaminima)
        {
            Prodotto = prodotto;
            QuantitaAttuale = quantitaAttuale;
            SogliaMinima = sogliaminima;
        }
    }
}