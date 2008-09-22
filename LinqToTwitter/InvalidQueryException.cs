/***********************************************************
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
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// custom exception for handling bad queries
    /// </summary>
    class InvalidQueryException : System.Exception
    {
        private string message;

        /// <summary>
        /// init exception
        /// </summary>
        /// <param name="message">message to display</param>
        public InvalidQueryException(string message)
        {
            this.message = message + " ";
        }

        /// <summary>
        /// message to display
        /// </summary>
        public override string Message
        {
            get
            {
                return "The client query is invalid: " + message;
            }
        }
    }
}
