using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitterPcl
{
    class Class1 : IQueryable
    {
        public IEnumerator GetEnumerator()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public Type ElementType { get; private set; }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider { get; private set; }
    }
}
