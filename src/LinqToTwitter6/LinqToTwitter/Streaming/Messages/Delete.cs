using System.Text.Json;

namespace LinqToTwitter
{
    public class Delete
    {
        public Delete() { }
        public Delete(JsonElement delete)
        {
            var del = delete.GetProperty("delete");
            var status = del.GetProperty("status");
            StatusID = status.GetProperty("id").GetUInt64();
            UserID = status.GetProperty("user_id").GetUInt64();
        }

        public ulong StatusID { get; set; }

        public ulong UserID { get; set; }
    }
}
