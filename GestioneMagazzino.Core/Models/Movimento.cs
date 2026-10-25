
namespace GestioneMagazzino.Models
{
    public class Movimento : IIdentificabile
    {
        public int IdMovimento { get; set; }
        public string CodiceProdotto { get; set; }
        public TipoMovimento Tipo { get; set; }
        public int Quantita { get; set; }
        public DateTime DataMovimento { get; set; }
        public string Identificatore => "MOV-"+IdMovimento;
        
        public Movimento(int idMovimento,string codiceprodotto,TipoMovimento tipo,int quantita, DateTime dataMovimento)
        {
            IdMovimento = idMovimento;
            CodiceProdotto = codiceprodotto;
            Tipo = tipo;
            Quantita = quantita;
            DataMovimento =dataMovimento;
        }
    }
}