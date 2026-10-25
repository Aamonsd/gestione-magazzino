using GestioneMagazzino.Models;
using Microsoft.Data.SqlClient;
 

namespace GestioneMagazzino.Repositories
{
    public class SqlProdottiRepository : IProdottiRepository
    {

        private readonly string stringaConnessione;

        public SqlProdottiRepository(string stringaConnessione)
        {
            this.stringaConnessione = stringaConnessione;
        }

        private static Prodotto LeggiProdotto(SqlDataReader lettore)
        {
            var prodotto = new Prodotto(lettore.GetString(0), lettore.GetString(1), lettore.GetDecimal(2), lettore.GetInt32(4));
            prodotto.QuantitaAttuale = lettore.GetInt32(3);
            return prodotto;
        }

        public void Aggiungi(Prodotto nuovo)
        {

            using var connessione = new SqlConnection(stringaConnessione);
            connessione.Open();

            using var comando = new SqlCommand(

                "INSERT INTO Prodotti(CodiceProdotto, Nome, PrezzoUnitario, QuantitaAttuale, SogliaMinima) " +
                "VALUES (@codice, @nome, @prezzo, @quantita, @soglia)", connessione

                );

            comando.Parameters.AddWithValue("@codice", nuovo.CodiceProdotto);
            comando.Parameters.AddWithValue("@nome", nuovo.Nome);
            comando.Parameters.AddWithValue("@prezzo", nuovo.PrezzoUnitario);
            comando.Parameters.AddWithValue("@quantita", nuovo.QuantitaAttuale);
            comando.Parameters.AddWithValue("@soglia", nuovo.SogliaMinima);

            comando.ExecuteNonQuery();

        }

        public List<Prodotto> OttieniTutti()
        {
            var prodotti = new List<Prodotto>();

            using var connessione = new SqlConnection(stringaConnessione);
            connessione.Open();

            using var comando = new SqlCommand(
                "SELECT CodiceProdotto, Nome, PrezzoUnitario, QuantitaAttuale, SogliaMinima FROM Prodotti", connessione);
            using var lettore = comando.ExecuteReader();

            while (lettore.Read())
            {
                prodotti.Add(LeggiProdotto(lettore));
            }
            return prodotti;
        }

        public Prodotto? TrovaPerCodice(string codice)
        {

            using var connessione = new SqlConnection(stringaConnessione);
            connessione.Open();

            using var comando = new SqlCommand(
                "SELECT CodiceProdotto, Nome, PrezzoUnitario, QuantitaAttuale, SogliaMinima FROM Prodotti WHERE CodiceProdotto=@codice", connessione
                );
            comando.Parameters.AddWithValue("@codice", codice);

            using var lettore = comando.ExecuteReader();
            if (lettore.Read())
            {
                return LeggiProdotto(lettore);
            }
            return null;
           

        }
    }
}
