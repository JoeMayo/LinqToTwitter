using System;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class Delete
    {
        public Delete() { }
        public Delete(JsonData delete)
        {
            var del = delete.GetValue<JsonData>("delete");
            var status = del.GetValue<JsonData>("status");
            StatusID = status.GetValue<ulong>("id");
            UserID = status.GetValue<ulong>("user_id");
        }

        public ulong StatusID { get; set; }

        public ulong UserID { get; set; }
    }
}
