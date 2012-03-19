// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidUnicodeValueException.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
//
// </copyright>
// <summary>
//   Thrown when a invalid Unicode valid is encountered.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Thrown when a invalid Unicode valid is encountered.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Usage",
        "CA2237:MarkISerializableTypesWithSerializable",
        Justification = "The exception does not leave the application domain and serialization breaks medium trust.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1032:ImplementStandardExceptionConstructors",
        Justification = "The exception does not leave the application domain and serialization breaks medium trust.")]
    public class InvalidUnicodeValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        public InvalidUnicodeValueException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidUnicodeValueException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public InvalidUnicodeValueException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        /// <param name="value">The invalid value.</param>
        public InvalidUnicodeValueException(int value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUnicodeValueException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="value">The invalid value.</param>
        public InvalidUnicodeValueException(string message, int value)
            : base(message)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the the invalid value.
        /// </summary>
        /// <value>The invalid value.</value>
        public int Value
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get
            {
                if (this.Value == 0)
                {
                    return base.Message;
                }

                return string.Format(CultureInfo.CurrentUICulture, "Value : {0:x4}", this.Value) + Environment.NewLine + "Message: " + base.Message;
            }
        }
    }
}
