using System;
using System.Windows.Input;

namespace Vision
{
    public class RelayCommand : ICommand
    {
        private Action _action;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action action, Func<bool> canExecute)
        {
            this._action = action;
            this._canExecute = canExecute;
        }

        public RelayCommand(Action action)
        {
            this._action = action;
            this._canExecute = () => true;
        }

        public bool CanExecute(object parameter)
        {
            bool result = this._canExecute.Invoke();
            return result;
        }

        public void Execute(object parameter)
        {
            this._action.Invoke();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    public class ParamCommand : ICommand
    {
        private Action<object> _action;
        private readonly Func<bool> _canExecute;

        public ParamCommand(Action<object> action)
        {
            _action = action;
            _canExecute = () => true;
        }

        public ParamCommand(Action<object> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (this._canExecute == null)
            {
                return true;
            }
            else
            {
                bool result = this._canExecute.Invoke();
                return result;
            }
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                if (parameter != null)
                {
                    _action(parameter);
                }
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }
}
