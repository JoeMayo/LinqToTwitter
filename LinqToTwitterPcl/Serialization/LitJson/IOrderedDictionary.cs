using System;
using System.Collections;
using System.Collections.Generic;
using LinqToTwitter;

namespace LitJson
{
    public interface IOrderedDictionary : IDictionary<string, JsonData>, ICollection, IEnumerable
    {
        object this[int index] { get; set; }

        new IDictionaryEnumerator GetEnumerator();

        void Insert(int index, object key, object value);

        void RemoveAt(int index);
    }
}
