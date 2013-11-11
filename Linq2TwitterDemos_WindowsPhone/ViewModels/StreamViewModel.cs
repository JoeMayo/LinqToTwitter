using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Linq2TwitterDemos_WindowsPhone.ViewModels
{
    class StreamViewModel
    {
        public StreamViewModel()
        {
            JsonContent = new ObservableCollection<JsonContent>();
        }

        public ObservableCollection<JsonContent> JsonContent { get; set; }
    }
}
