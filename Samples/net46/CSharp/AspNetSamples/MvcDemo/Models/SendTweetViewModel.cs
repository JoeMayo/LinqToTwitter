using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MvcDemo.Models
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
