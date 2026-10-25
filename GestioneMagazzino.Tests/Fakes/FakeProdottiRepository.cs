using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Models;
using GestioneMagazzino.Repositories;

namespace GestioneMagazzino.Tests.Fakes
{
    public class FakeProdottiRepository:IProdottiRepository
    {

        private readonly List<Prodotto> prodotti = new();

        public void Aggiungi(Prodotto nuovo)
        {
            prodotti.Add(nuovo);
        }

        public Prodotto? TrovaPerCodice(string codice)
        {
            return prodotti.FirstOrDefault(p => string.Equals(p.CodiceProdotto, codice, StringComparison.OrdinalIgnoreCase));

        }

        public List<Prodotto> OttieniTutti()
        {
            return new List<Prodotto>(prodotti);
        }

        
    }
}
