/*****************************************************************
 * Credits:
 * 
 * MSDN Documentation -
 * Walkthrough: Creating an IQueryable LINQ Provider
 * 
 * http://msdn.microsoft.com/en-us/library/bb546158.aspx
 * 
 * Matt Warren's Blog -
 * LINQ: Building an IQueryable Provider:
 * 
 * http://blogs.msdn.com/mattwar/default.aspx
 * 
 * Modified By: Joe Mayo, 5/2/09 -
 * 
 * Refactored as standard exception: 
 * 
 *      - derives from Application
 *      - has standard exception constructors
 *      - beyond the type name, it looks nothing like the original
 *****************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// custom exception for handling bad queries
    /// </summary>
    public class InvalidQueryException : ApplicationException
    {
        /// <summary>
        /// init exception with general message - 
        /// you should probably use one of the other
        /// constructors for a more meaninful exception.
        /// </summary>
        public InvalidQueryException()
            : this("Invalid query: reason not specified.", null) { }

        /// <summary>
        /// init exception with custom message
        /// </summary>
        /// <param name="message">message to display</param>
        public InvalidQueryException(string message)
            : base (message, null) { }

        /// <summary>
        /// init exception with custom message and chain to originating exception
        /// </summary>
        /// <param name="message">custom message</param>
        /// <param name="inner">originating exception</param>
        public InvalidQueryException(string message, Exception inner)
            : base(message, inner) { }
    }
}
