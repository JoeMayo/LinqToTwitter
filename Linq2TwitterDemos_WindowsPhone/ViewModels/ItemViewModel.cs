using System;
using System.ComponentModel;

namespace Linq2TwitterDemos_WindowsPhone.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string operation;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string Operation
        {
            get
            {
                return operation;
            }
            set
            {
                if (value != operation)
                {
                    operation = value;
                    NotifyPropertyChanged("Operation");
                }
            }
        }

        private string pageUri;
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public string PageUri
        {
            get
            {
                return pageUri;
            }
            set
            {
                if (value != pageUri)
                {
                    pageUri = value;
                    NotifyPropertyChanged("PageUri");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}