using System;
using System.Windows.Input;

namespace UwpSamples
{
    class  TwitterCommand<T> : ICommand
    {
        readonly Action<T> callback;

        public TwitterCommand(Action<T> handler)
        {
            callback = handler;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (callback != null) callback((T)parameter);
        }
    }
}
