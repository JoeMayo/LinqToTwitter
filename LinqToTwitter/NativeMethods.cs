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
using System.Runtime.InteropServices;

namespace LinqToTwitter
{
    /// <summary>
    /// P/Invoke methods
    /// </summary>
    internal class NativeMethods
    {
        /// <summary>
        /// Finds a window by its caption.
        /// </summary>
        /// <param name="zeroOnly">Must be set to <see cref="IntPtr.Zero"/>.</param>
        /// <param name="windowName">The title of the window to find.</param>
        /// <returns>The HWND of the window found; or <see cref="IntPtr.Zero"/> if not found.</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        internal static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string windowName);
    }
}