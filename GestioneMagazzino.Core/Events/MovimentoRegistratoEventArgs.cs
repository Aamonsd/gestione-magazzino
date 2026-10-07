 
using GestioneMagazzino.Models;

namespace GestioneMagazzino.Events
{
    public class MovimentoRegistratoEventArgs: EventArgs
    {
         public Movimento Movimento { get; }

        public MovimentoRegistratoEventArgs(Movimento movimento)
        {
            Movimento = movimento;
        }

    }
}