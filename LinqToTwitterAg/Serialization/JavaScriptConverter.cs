using System;
using System.Collections.Generic;

namespace LinqToTwitter.Json
{
    public abstract class JavaScriptConverter
    {
        public abstract IEnumerable<Type> SupportedTypes { get; }

        public abstract Object Deserialize(
            IDictionary<string, Object> dictionary,
            Type type,
            JavaScriptSerializer serializer
        );

        public abstract IDictionary<string, Object> Serialize(
            Object obj,
            JavaScriptSerializer serializer
        );
    }
}
