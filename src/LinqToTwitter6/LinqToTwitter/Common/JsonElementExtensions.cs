using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LinqToTwitter.Common
{
    public static class JsonElementExtensions
    {
        public static bool IsNull(this JsonElement json)
        {
            return json.ValueKind == JsonValueKind.Undefined || json.ValueKind == JsonValueKind.Null;
        }
    }
}
