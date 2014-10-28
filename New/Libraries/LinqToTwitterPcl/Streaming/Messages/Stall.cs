using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class Stall
    {
        public Stall() { }
        public Stall(JsonData stall)
        {
            var warning = stall.GetValue<JsonData>("user_withheld");
            Code = warning.GetValue<string>("code");
            Message = warning.GetValue<string>("message");
            PercentFull = warning.GetValue<int>("percent_full");
        }

        public string Code { get; set; }

        public string Message { get; set; }

        public int PercentFull { get; set; }
    }
}
