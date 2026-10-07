using System.ComponentModel;

namespace GestioneMagazzino.Wpf.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string nomeProprieta)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nomeProprieta));
        }

    }
}
