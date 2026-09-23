
namespace GestioneMagazzino.Models
{
    public class Prodotto:IIdentificabile
    {
        public string CodiceProdotto{ get; set; }
        public string Nome { get; set; }
        public decimal PrezzoUnitario { get; set; }
        public int QuantitaAttuale { get; set; }
        public int SogliaMinima { get; set; }
        public string Identificatore => CodiceProdotto;

        public Prodotto(string codiceProdotto,string nome,decimal prezzoUnitario,int sogliaMinima)
        {
            CodiceProdotto = codiceProdotto;
            Nome = nome;
            PrezzoUnitario = prezzoUnitario;
            SogliaMinima = sogliaMinima;
            QuantitaAttuale = 0;
        }
        public bool SottoSoglia()
        {
            if (QuantitaAttuale < SogliaMinima) return true;
            else return false;
        }

        public decimal ValoreScorta()
        {
            return QuantitaAttuale * PrezzoUnitario;
        }
    }
}