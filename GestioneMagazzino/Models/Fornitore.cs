
namespace GestioneMagazzino.Models
{
    public class Fornitore:IIdentificabile
    {
        public string CodiceFornitore { get; set; }
        public string RagioneSociale { get; set; }
        public string Email { get; set; }
        public string Identificatore => CodiceFornitore;

        public Fornitore(string codiceFornitore,string ragioneSociale,string email)
        {
            CodiceFornitore = codiceFornitore;
            RagioneSociale = ragioneSociale;
            Email = email;
        }
    }
}