using System;
using System.Linq;
using System.Windows.Input;

namespace Linq2TwitterDemos_WindowsStore.DataModel
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
