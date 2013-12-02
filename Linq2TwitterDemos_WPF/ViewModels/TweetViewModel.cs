using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Linq2TwitterDemos_WPF.ViewModels
{
    public class TweetViewModel
    {
        public TweetViewModel()
        {
            Tweets = new ObservableCollection<Tweet>();
        }

        public ObservableCollection<Tweet> Tweets { get; set; }
    }
}
