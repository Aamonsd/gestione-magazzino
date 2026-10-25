using GestioneMagazzino.Models;

namespace GestioneMagazzino.Repositories
{
    public interface IMovimentiRepository
    {
        List<Movimento> OttieniTutti();
        Movimento Registra(string codiceProdotto,TipoMovimento tipo,int quantita);
    }
}
