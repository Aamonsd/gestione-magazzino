using System;
using System.Collections.Generic;
using System.Text;
using GestioneMagazzino.Models;

namespace GestioneMagazzino.Core.Repositories
{
    public interface IProdottiRepository
    {
        void Aggiungi(Prodotto nuovo);
        Prodotto? TrovaPerCodice(string codice);
        List<Prodotto> OttieniTutti();
    }
}
