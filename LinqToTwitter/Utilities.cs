//-----------------------------------------------------------------------
// <copyright file="Utilities.cs">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
// <license>
//     Microsoft Public License (Ms-PL http://opensource.org/licenses/ms-pl.html).
//     Contributors may add their own copyright notice above.
// </license>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using IWin32Window = System.Windows.Forms.IWin32Window;
using System.Threading;

namespace LinqToTwitter
{
    public static class Utilities
    {
        /// <summary>
        /// Wraps an HWND IntPtr as an IWin32Window so that it can be used as the parent window to popup dialogs.
        /// </summary>
        private class HWndWrapper : IWin32Window
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HWndWrapper"/> class.
            /// </summary>
            /// <param name="hwnd">The HWND.</param>
            internal HWndWrapper(IntPtr hwnd)
            {
                this.Handle = hwnd;
            }

            #region IWin32Window Members

            /// <summary>
            /// Gets the handle to the window represented by the implementer.
            /// </summary>
            /// <value></value>
            /// <returns>
            /// A handle to the window represented by the implementer.
            /// </returns>
            public IntPtr Handle { get; private set; }

            #endregion
        }

        /// <summary>
        /// Gets the HWND of the application's console window.
        /// </summary>
        public static IWin32Window GetConsoleHWnd()
        {
            string originalTitle = Console.Title;
            string uniqueTitle = Guid.NewGuid().ToString();
            Console.Title = uniqueTitle;
            try
            {
                Thread.Sleep(50);
                IntPtr handle = NativeMethods.FindWindowByCaption(IntPtr.Zero, uniqueTitle);

                if (handle == IntPtr.Zero)
                {
                    throw new ApplicationException("Could not obtain console window HWND.");
                }

                return new HWndWrapper(handle);
            }
            finally
            {
                Console.Title = originalTitle;
            }
        }

        /// <summary>
        /// Gets the IWin32Window compatible wrapper for a WPF window.
        /// </summary>
        public static IWin32Window GetWin32Window(this Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            return new HWndWrapper(new WindowInteropHelper(window).Handle);
        }

        /// <summary>
        /// Converts a SecureString to an ordinary String.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The ordinary String.</returns>
        /// <remarks>
        /// It is important to note that converting a SecureString to an ordinary
        /// string eliminates any added security that using a SecureString provides.
        /// </remarks>
        internal static string ToUnsecureString(this SecureString value)
        {
            if (value == null)
            {
                return null;
            }

            IntPtr bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        /// <summary>
        /// Creates a SecureString instance from an ordinary string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The SecureString.</returns>
        /// <remarks>
        /// It is important to note that although a SecureString instance is created, the entire
        /// process is insecure by nature since we built it out of an insecure string which remains
        /// in memory indefinitely.
        /// </remarks>
        internal static SecureString ToSecureString(this string value)
        {
            if (value == null)
            {
                return null;
            }

            SecureString secureString = new SecureString();
            Array.ForEach(value.ToCharArray(), s => secureString.AppendChar(s));
            return secureString;
        }

        /// <summary>
        /// Creates a new Uri based on a given Uri, with an appended query string containing all the given parameters.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A new Uri instance.</returns>
        internal static Uri AppendQueryString(Uri requestUri, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }

            if (parameters == null)
            {
                return requestUri;
            }

            UriBuilder builder = new UriBuilder(requestUri);
            if (!string.IsNullOrEmpty(builder.Query))
            {
                builder.Query += "&" + BuildQueryString(parameters);
            }
            else
            {
                builder.Query = BuildQueryString(parameters);
            }

            return builder.Uri;
        }

        /// <summary>
        /// Assembles a series of key=value pairs as a URI-escaped query-string.
        /// </summary>
        /// <param name="parameters">The parameters to include.</param>
        /// <returns>A query-string-like value such as a=b&c=d.  Does not include a leading question mark (?).</returns>
        internal static string BuildQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            StringBuilder builder = new StringBuilder();
            foreach (var pair in parameters.Where(p => !string.IsNullOrEmpty(p.Value)))
            {
                if (builder.Length > 0)
                {
                    builder.Append("&");
                }

                builder.Append(Uri.EscapeDataString(pair.Key));
                builder.Append("=");
                builder.Append(Uri.EscapeDataString(pair.Value));
            }

            return builder.ToString();
        }
    }
}
