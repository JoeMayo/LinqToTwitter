using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public interface IDictionaryEnumerator : IEnumerator
    {
        object Key { get; }

        DictionaryEntry Entry { get; }

        object Value { get; }
    }
}
