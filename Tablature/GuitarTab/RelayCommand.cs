using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab
{
    public class RelayCommand : ICommand
    {
        private Predicate<object> canExecute;
        private Action execute;

        public RelayCommand(Action ex, Predicate<object> canEx = null)
        {
            execute = ex;
            canExecute = canEx;
        }

        public bool CanExecute(object parameter)
        {
            return (canExecute == null) ? true : canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
