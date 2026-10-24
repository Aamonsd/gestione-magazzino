using GestioneMagazzino.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestioneMagazzino.Core.Repositories
{
    public interface IMovimentiRepository
    {
        List<Movimento> OttieniTutti();
        Movimento Registra(string codiceProdotto,TipoMovimento tipo,int quantita);
    }
}
