#nullable disable
using System.Text.Json;

namespace LinqToTwitter
{
    public class Control
    {
        public Control() { }
        public Control(JsonElement control)
        {
            var ctrl = control.GetProperty("control");
            URL = ctrl.GetProperty("control_uri").GetString();
        }

        public string URL { get; set; }
    }
}
