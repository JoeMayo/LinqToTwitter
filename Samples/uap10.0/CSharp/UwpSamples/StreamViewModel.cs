using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace UwpSamples
{
    abstract class StreamViewModel : INotifyPropertyChanged
    {        
        public StreamViewModel()
        {
            JsonContent = new ObservableCollection<JsonContent>();
            StartCommand = new TwitterCommand<object>(OnStart);
        }

        public ObservableCollection<JsonContent> JsonContent { get; set; }

        public TwitterCommand<object> StartCommand { get; set; }

        public abstract void OnStart(object obj);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected async Task ShowAsync(string content)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                JsonContent.Insert(0, new JsonContent { Content = content });
            });            
        }
    }
}
