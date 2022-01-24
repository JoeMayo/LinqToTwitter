using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LinqToTwitter.MVC.CSharp.Models
{
    public class MediaViewModel
    {
        [DisplayName("Image")]
        [DataType(DataType.ImageUrl)]
        public string MediaUrl { get; set; }

        [DisplayName("Screen Name")]
        public string Description { get; set; }

        [DisplayName("Tweet")]
        public string Text { get; set; }
    }
}
