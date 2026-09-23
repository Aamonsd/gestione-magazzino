# Gestione Magazzino

Applicazione console in C# per la gestione di un magazzino: prodotti, movimenti di carico/scarico e controllo automatico delle scorte minime.

## Cosa fa

- Aggiunta di nuovi prodotti al magazzino, con codice, nome, prezzo unitario e soglia minima di riordino
- Registrazione dei movimenti di magazzino (carico e scarico), con aggiornamento automatico della quantità disponibile
- Blocco automatico degli scarichi quando la quantità richiesta supera quella disponibile
- Notifica automatica quando un prodotto scende sotto la soglia minima impostata
- Storico completo di tutti i movimenti effettuati
- Salvataggio e caricamento automatico dei dati su file, in modo che le informazioni non vadano perse alla chiusura del programma

## Tecnologie

- C# / .NET 8
- Persistenza dati in formato JSON (`System.Text.Json`)

## Come si esegue

1. Aprire `GestioneMagazzino.sln` in Visual Studio (o editor equivalente)
2. Impostare `GestioneMagazzino` come progetto di avvio
3. Eseguire il progetto

Al primo avvio verranno creati automaticamente due file, `prodotti.json` e `movimenti.json`, nella cartella di esecuzione: da quel momento i dati inseriti vengono ricaricati automaticamente ad ogni riavvio del programma.

## Struttura del progetto

```
GestioneMagazzino/
├── Models/          Entità del dominio (Prodotto, Movimento, Fornitore, enum, interfacce)
├── Events/          Classi EventArgs per la comunicazione basata su eventi
├── Repositories/    Repository generico per la gestione e persistenza dei dati
├── Exceptions/       Eccezioni custom per i casi di errore del dominio
├── Services/        Logica di business dell'applicazione
└── Program.cs       Punto di ingresso e interfaccia a menu
```

## Concetti C# dimostrati

- **Programmazione a oggetti**: interfacce (`IIdentificabile`), incapsulamento, relazioni tra entità tramite identificatore (pattern simile a una chiave esterna di database, per evitare duplicazione dei dati tra `Prodotto` e `Movimento`)
- **Generics**: `Repository<T>` è una classe generica riutilizzabile per qualsiasi entità che implementi `IIdentificabile`, usata sia per i prodotti sia per i movimenti senza duplicare codice
- **Eventi e delegati**: il `ServizioMagazzino` espone eventi (`MovimentoRegistrato`, `ScortaBassa`) a cui l'interfaccia utente si iscrive, secondo il pattern publisher/subscriber, mantenendo la logica di business disaccoppiata dalla presentazione
- **Gestione delle eccezioni**: eccezioni custom (`ScortaInsufficienteException`) per segnalare in modo esplicito gli errori di dominio, gestite senza mai interrompere l'esecuzione del programma
- **File handling e serializzazione**: salvataggio e caricamento dello stato dell'applicazione tramite serializzazione JSON

## Possibili sviluppi futuri

- Interfaccia grafica (WPF)
- Test automatizzati (xUnit)
- Gestione fornitori collegata ai prodotti
