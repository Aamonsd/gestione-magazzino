using GestioneMagazzino.Models;

namespace GestioneMagazzino.Wpf.ViewModels
{
    public record MovimentoVisualizzato(string NomeProdotto, TipoMovimento Tipo, int Quantita);
}
