
using GestioneMagazzino.Events;
using GestioneMagazzino.Exceptions;
using GestioneMagazzino.Models;
using GestioneMagazzino.Services;





namespace GestioneMagazzino
{
    public class Program
    {

        
        private static ServizioMagazzino servizio = new ServizioMagazzino();
        public static void MovimentoRegistratoHandler(object sender,MovimentoRegistratoEventArgs e)
        {
            Prodotto prodotto = servizio.TrovaProdotto(e.Movimento.CodiceProdotto);
            Console.WriteLine($"Movimento Registrato: {e.Movimento.Tipo} di {e.Movimento.Quantita} per {prodotto.Nome} ");
        }
        
        public static void ScortaBassaHandler(object sender,ScortaBassaEventArgs e)
        {
            Console.WriteLine($"ATTENZIONE: {e.Prodotto.Nome} sotto soglia! Attuale: {e.QuantitaAttuale}, soglia: {e.SogliaMinima}");
        }

        public static void Main(string[] args)
        {

            
            servizio.MovimentoRegistrato += MovimentoRegistratoHandler;
            servizio.ScortaBassa += ScortaBassaHandler;
            string choose = "";
            string rispostaTipo = "";
            TipoMovimento tipo = TipoMovimento.Carico;
            servizio.CaricaDati();
            

            Console.WriteLine
            ("=== GESTIONE MAGAZZINO ===\n" +
            "1. Aggiungi prodotto\n" +
            "2. Registra movimento (carico/scarico)\n" +
            "3. Visualizza tutti i prodotti\n" +
            "4. Visualizza storico movimenti\n" +
            "5. Esci\n"+
            "Scegli un'opzione"
            );

            do
            {
                choose = Console.ReadLine();
                switch (choose)
                {
                    case "1":

                        try
                        {
                            Console.WriteLine("Inserisci codice prodotto:");
                            string codice = Console.ReadLine();
                            Console.WriteLine("Inserisci nome prodotto:");
                            string nome = Console.ReadLine();
                            Console.WriteLine("Inserisci prezzo unitario:");
                            decimal prezzo = decimal.Parse(Console.ReadLine());
                            Console.WriteLine("Inserisci soglia minima:");
                            int soglia = int.Parse(Console.ReadLine());

                            Prodotto prodotto = new Prodotto(codice, nome, prezzo, soglia);
                            servizio.AggiungiProdotto(prodotto);

                            Console.WriteLine("Prodotto aggiunto");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Errore: prezzo e soglia devono essere numeri validi (es. prezzo 2,50 e soglia 10)");
                        }
                        catch (ProdottoGiaEsistenteException ex)
                        {
                            Console.WriteLine("Errore: " + ex.Message);
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine("Errore: " + ex.Message);
                        }
                        break;



                    case "2":
                        try
                        {
                            Console.WriteLine("Inserisci Codice Prodotto");
                            Prodotto prodottoTrovato = servizio.TrovaProdotto(Console.ReadLine());

                            do
                            {
                                Console.WriteLine("inserisci tipo di movimento carico o scarico");
                                rispostaTipo = Console.ReadLine();
                                if (string.IsNullOrEmpty(rispostaTipo))
                                {
                                    continue;
                                }
                                rispostaTipo = char.ToUpper(rispostaTipo[0]) + rispostaTipo[1..].Trim().ToLower();
                                
                                tipo = TipoMovimento.Carico;

                                switch (rispostaTipo)
                                {
                                    case "Carico":
                                        tipo = TipoMovimento.Carico;
                                        break;
                                    case "Scarico":
                                        tipo = TipoMovimento.Scarico;
                                        break;

                                    default:
                                        Console.WriteLine("tipo di movimento non valido");
                                        break;

                                }

                            } while (rispostaTipo != "Carico" && rispostaTipo != "Scarico");
                             
                            
                            Console.WriteLine("inserisci la quantita");
                            int quantita = int.Parse(Console.ReadLine());
                            servizio.RegistraMovimento(prodottoTrovato, tipo, quantita);

                        }
                        catch(InvalidOperationException)
                        {
                            Console.WriteLine("Errore: Prodotto non trovato");
                        }
                        catch (ScortaInsufficienteException ex)
                        {
                            Console.WriteLine("Errore: "+ex.Message);
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine("Errore: "+ex.Message);
                        }
                        
                        
                        break;

                    case "3":
                        Console.WriteLine("visualizza prodotti");
                        List<Prodotto> listaProdotti=servizio.OttieniProdotti();

                        if (listaProdotti.Count == 0)
                        {
                            Console.WriteLine("Nessun dato disponibile");
                        }

                        foreach (var item in listaProdotti)
                        {
                            Console.WriteLine($"ID: {item.CodiceProdotto}, Nome: {item.Nome}, Prezzo unitario: {item.PrezzoUnitario}, Soglia:{item.SogliaMinima} ");
                        }
                        break;

                    case "4":
                        Console.WriteLine("visualizza movimenti");
                        List<Movimento> storicoMovimenti = servizio.OttieniMovimenti();
                       
                        if (storicoMovimenti.Count == 0)
                        {
                            Console.WriteLine("Nessun dato disponibile");
                        }

                        foreach (var item in storicoMovimenti)
                        {
                            Console.WriteLine($"Nome Prodotto: {servizio.TrovaProdotto(item.CodiceProdotto).Nome}, tipo Movimento: {item.Tipo} ,quantita: {item.Quantita}");
                        }
                        break;

                    case "5":
                        break;

                    default:
                        Console.WriteLine("comando sbagliato");
                        break;
                }
                
                    

            } while (choose != "5");


        }
    }
}
