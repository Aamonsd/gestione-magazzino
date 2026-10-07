using GestioneMagazzino.Models;

namespace GestioneMagazzino.Wpf.ViewModels
{
    public record MovimentoStorico(DateTime Data, string IdMovimento, string NomeProdotto, TipoMovimento Tipo, int Quantita);
}
