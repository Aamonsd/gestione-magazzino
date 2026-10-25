using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Models;
using Microsoft.Data.SqlClient;
 
namespace GestioneMagazzino.Repositories
{
    public class SqlMovimentiRepository:IMovimentiRepository
    {

        private readonly string stringaConnessione;

        public SqlMovimentiRepository(string stringaConnessione)
        {
            this.stringaConnessione = stringaConnessione;
        }

        private static Movimento LeggiMovimento(SqlDataReader lettore)
        {
            var movimento = new Movimento(lettore.GetInt32(0), lettore.GetString(1),
                (TipoMovimento)lettore.GetByte(2), lettore.GetInt32(3),
                DateTime.SpecifyKind(lettore.GetDateTime(4), DateTimeKind.Utc).ToLocalTime());

            return movimento;
                 
        }

        public List<Movimento> OttieniTutti()
        {
            var movimenti = new List<Movimento>();

            using var connessione = new SqlConnection(stringaConnessione);
            connessione.Open();

            using var comando = new SqlCommand(
                "SELECT IdMovimento,CodiceProdotto,Tipo,Quantita,DataMovimento FROM Movimenti",connessione
                );
            using var lettore = comando.ExecuteReader();
            while (lettore.Read())
            {
                movimenti.Add(LeggiMovimento(lettore));
            }
            return movimenti;
            

        }

        public Movimento Registra(string codiceProdotto, TipoMovimento tipo, int quantita)
        {
            using var connessione = new SqlConnection(stringaConnessione);
            connessione.Open();
            using var transazione = connessione.BeginTransaction();

            // 1. Aggiorna la scorta: + per il carico, - per lo scarico
            int variazione = tipo == TipoMovimento.Carico ? quantita : -quantita;

            using var aggiorna = new SqlCommand(
                "UPDATE Prodotti SET QuantitaAttuale = QuantitaAttuale + @variazione " +
                "WHERE CodiceProdotto = @codice AND QuantitaAttuale + @variazione >= 0",
                 connessione, transazione);
            aggiorna.Parameters.AddWithValue("@variazione", variazione);
            aggiorna.Parameters.AddWithValue("@codice", codiceProdotto);

            if (aggiorna.ExecuteNonQuery() == 0)
                throw new ScortaInsufficienteException($"Scorta insufficiente per {codiceProdotto}");

            using var inserisci = new SqlCommand(
                "INSERT INTO Movimenti (CodiceProdotto, Tipo, Quantita) "+
                "OUTPUT INSERTED.IdMovimento, INSERTED.CodiceProdotto, INSERTED.Tipo, INSERTED.Quantita, INSERTED.DataMovimento "+
                "VALUES (@codice, @tipo, @quantita) ",connessione,transazione);

            inserisci.Parameters.AddWithValue("@codice", codiceProdotto);
            inserisci.Parameters.AddWithValue("@tipo", (byte)tipo);
            inserisci.Parameters.AddWithValue("@quantita", quantita);
            Movimento movimento;
            using (var lettore = inserisci.ExecuteReader())
            {
                lettore.Read();
                movimento = LeggiMovimento(lettore);
            }
            transazione.Commit();
            return movimento;

        }
    }
}
