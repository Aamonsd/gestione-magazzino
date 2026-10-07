using System.Windows.Input;

namespace GestioneMagazzino.Wpf.Commands
{
    public class RelayCommand:ICommand
    {
        private readonly Action azione;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action azione)
        {
            this.azione = azione;
        }

        public void Execute(object? parameter)
        {
            azione();
        }


        public bool CanExecute(object? parameter)
        {
            return true;
        }
        
       

        

    }
}
