using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LinqToTwitter.MVC.CSharp.Models
{
    public class SendTweetViewModel
    {
        [DisplayName("Tweet Text:")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }


        public string Response { get; set; }
    }
}
