
namespace GestioneMagazzino.Models
{
    public class Movimento : IIdentificabile
    {
        public string IdMovimento { get; set; }
        public string CodiceProdotto { get; set; }
        public TipoMovimento Tipo { get; set; }
        public int Quantita { get; set; }
        public DateTime DataMovimento { get; set; }
        public string Identificatore => IdMovimento;
        
        public Movimento(string idMovimento,string codiceprodotto,TipoMovimento tipo,int quantita)
        {
            IdMovimento = idMovimento;
            CodiceProdotto = codiceprodotto;
            Tipo = tipo;
            Quantita = quantita;
            DataMovimento = DateTime.Now;
        }
    }
}