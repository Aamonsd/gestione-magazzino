using GestioneMagazzino.Models;

namespace GestioneMagazzino.Repositories
{
    public interface IProdottiRepository
    {
        void Aggiungi(Prodotto nuovo);
        Prodotto? TrovaPerCodice(string codice);
        List<Prodotto> OttieniTutti();
    }
}
