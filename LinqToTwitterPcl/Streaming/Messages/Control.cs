using System;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class Control
    {
        public Control() { }
        public Control(JsonData control)
        {
            var ctrl = control.GetValue<JsonData>("control");
            URL = control.GetValue<string>("control_uri");
        }

        public string URL { get; set; }
    }
}
