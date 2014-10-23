using System;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class Event
    {
        public Event() { }
        public Event(JsonData evt)
        {
            Target = evt.GetValue<string>("target");
            Source = evt.GetValue<string>("source");
            EventName = evt.GetValue<string>("event");
            TargetObject = evt.GetValue<string>("target_object");
            CreatedAt = evt.GetValue<string>("created_at").GetDate(DateTime.MaxValue);
        }

        public string Target { get; set; }

        public string Source { get; set; }

        public string EventName { get; set; }

        public string TargetObject { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
