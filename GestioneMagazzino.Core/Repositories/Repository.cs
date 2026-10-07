
using GestioneMagazzino.Models;
using System.Text.Json;
namespace GestioneMagazzino.Repositories
{

    public class Repository<T>where T:IIdentificabile
    {
        private List<T> lista=new List<T>() { };

        public void Aggiungi(T elemento)
        {
            lista.Add(elemento);
        }
        public T TrovaPerIdentificatore(string id)
        {
            foreach (var item in lista)
            {
                if (string.Equals(item.Identificatore, id, StringComparison.OrdinalIgnoreCase)) return item;
            }
            throw new InvalidOperationException("elemento non trovato");
        }

        public void RimuoviPerIdentificatore(string id)
        {

            T? elementoTrovato = default;
            foreach (var item in lista)
            {
                if (string.Equals(item.Identificatore, id, StringComparison.OrdinalIgnoreCase))
                {
                    elementoTrovato = item;
                    break;
                }
            }

            if (elementoTrovato == null)
            {
                throw new ArgumentException("elemento non trovato");
            }
            lista.Remove(elementoTrovato);
        }

        public List<T> OttieniTutti()
        {
            List<T> listaNuova = new List<T>(lista);
            return listaNuova;
        }

        public void SalvaFile(string percorso)
        {
            
            string json = JsonSerializer.Serialize(lista);
            File.WriteAllText(percorso,json);

        }

        public void CaricaFile(string percorso)
        {
            if (!File.Exists(percorso)) return;

            string json = File.ReadAllText(percorso);
            List<T>? listaCaricata = JsonSerializer.Deserialize<List<T>>(json);
            if (listaCaricata == null)
            {
                lista = new List<T>();
            }
            else
            {
                lista = listaCaricata;
            }
        }


    }
}