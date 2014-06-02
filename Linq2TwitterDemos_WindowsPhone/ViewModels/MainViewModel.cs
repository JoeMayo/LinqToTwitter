using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Linq2TwitterDemos_WindowsPhone.Resources;

namespace Linq2TwitterDemos_WindowsPhone.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>();
            Statuses = new ObservableCollection<ItemViewModel>();
            Streams = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        /// <summary>
        /// A collection for ItemViewModel objects with Statuses.
        /// </summary>
        public ObservableCollection<ItemViewModel> Statuses { get; private set; }

        /// <summary>
        /// A collection for ItemViewModel objects with Statuses.
        /// </summary>
        public ObservableCollection<ItemViewModel> Streams { get; private set; }

        private string sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return sampleProperty;
            }
            set
            {
                if (value != sampleProperty)
                {
                    sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get
            {
                return AppResources.SampleProperty;
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            Statuses.Add(new ItemViewModel { Operation = "tweet", PageUri = "/StatusDemos/TweetDemo.xaml" });
            Statuses.Add(new ItemViewModel { Operation = "home timeline", PageUri = "/StatusDemos/HomeTimelineDemo.xaml" });

            Streams.Add(new ItemViewModel { Operation = "filter stream", PageUri = "/StreamingDemos/FilterStreamDemo.xaml" });
            Streams.Add(new ItemViewModel { Operation = "sample stream", PageUri = "/StreamingDemos/SampleStreamDemo.xaml" });
            Streams.Add(new ItemViewModel { Operation = "user stream", PageUri = "/StreamingDemos/UserStreamDemo.xaml" });

            // Sample data; replace with real data
            this.Items.Add(new ItemViewModel() { Operation = "runtime one", PageUri = "Maecenas praesent accumsan bibendum" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime two", PageUri = "Dictumst eleifend facilisi faucibus" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime three", PageUri = "Habitant inceptos interdum lobortis" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime four", PageUri = "Nascetur pharetra placerat pulvinar" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime five", PageUri = "Maecenas praesent accumsan bibendum" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime six", PageUri = "Dictumst eleifend facilisi faucibus" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime seven", PageUri = "Habitant inceptos interdum lobortis" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime eight", PageUri = "Nascetur pharetra placerat pulvinar" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime nine", PageUri = "Maecenas praesent accumsan bibendum" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime ten", PageUri = "Dictumst eleifend facilisi faucibus" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime eleven", PageUri = "Habitant inceptos interdum lobortis" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime twelve", PageUri = "Nascetur pharetra placerat pulvinar" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime thirteen", PageUri = "Maecenas praesent accumsan bibendum" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime fourteen", PageUri = "Dictumst eleifend facilisi faucibus" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime fifteen", PageUri = "Habitant inceptos interdum lobortis" });
            this.Items.Add(new ItemViewModel() { Operation = "runtime sixteen", PageUri = "Nascetur pharetra placerat pulvinar" });

            this.IsDataLoaded = true;
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