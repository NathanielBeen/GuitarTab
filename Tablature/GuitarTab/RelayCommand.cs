using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

    public class MouseRelayCommand : ICommand
    {
        private Predicate<object> canExecute;
        private Action<Point> execute;

        public MouseRelayCommand(Action<Point> ex, Predicate<object> canEx = null)
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
            execute((Point)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class DimensionRelayCommand : ICommand
    {
        private Predicate<object> canExecute;
        private Action<DimensionChangedEventArgs> execute;

        public DimensionRelayCommand(Action<DimensionChangedEventArgs> ex, Predicate<object> canEx = null)
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
            execute((DimensionChangedEventArgs)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
