// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LdapEncoder.cs" company="Microsoft Corporation">
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
//   Provides LDAP Encoding methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using LinqToTwitter.Common;

namespace LinqToTwitter.Security.Application
{
    using System.Collections;
    using System.Text;

    /// <summary>
    /// Provides LDAP Encoding methods.
    /// </summary>
    internal static class LdapEncoder
    {
        /// <summary>
        /// A lock object to use when performing filter safe listing initialization.
        /// </summary>
        private static readonly ReaderWriterLock FilterSafeListSyncLock = new ReaderWriterLock();

        /// <summary>
        /// A lock object to use when performing DN safe listing initialization.
        /// </summary>
        private static readonly ReaderWriterLock DistinguishedNameSafeListSyncLock = new ReaderWriterLock();

        /// <summary>
        /// The values to output for each character when filter encoding.
        /// </summary>
        private static char[][] filterCharacterValues;

        /// <summary>
        /// The values to output for each character when DN encoding.
        /// </summary>
        private static char[][] distinguishedNameCharacterValues;

        /// <summary>
        /// Encodes the input string for use in LDAP filters.
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <returns>An encoded version of the input string suitable for use in LDAP filters.</returns>
        internal static string FilterEncode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (filterCharacterValues == null)
            {
                InitialiseFilterSafeList();
            }

            // RFC 4515 states strings must be converted to their UTF8 value before search filter encoding.
            // See http://tools.ietf.org/html/rfc4515
            // Conversion to char[] keeps null characters inline.
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(input.ToCharArray());
            char[] encodedInput = new char[utf8Bytes.Length * 3]; // Each byte can potentially be encoded as %xx
            int outputLength = 0;

            FilterSafeListSyncLock.EnterReadLock();
            try
            {
                for (int characterPosition = 0; characterPosition < utf8Bytes.Length; characterPosition++)
                {
                    byte currentCharacter = utf8Bytes[characterPosition];

                    if (filterCharacterValues[currentCharacter] != null)
                    {
                        // Character needs encoding.
                        char[] encodedCharacter = filterCharacterValues[currentCharacter];

                        for (int j = 0; j < encodedCharacter.Length; j++)
                        {
                            encodedInput[outputLength++] = encodedCharacter[j];
                        }
                    }
                    else
                    {
                        // Character does not need encoding.
                        encodedInput[outputLength++] = (char)currentCharacter;
                    }
                }
            }
            finally
            {
                FilterSafeListSyncLock.ExitReadLock();
            }

            return new string(encodedInput, 0, outputLength);
        }

        /// <summary>
        /// Encodes the input string for use in LDAP DNs.
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <param name="useInitialCharacterRules">Value indicating whether the special case rules for encoding of spaces and octothorpes at the start of a string are used.</param>
        /// <param name="useFinalCharacterRule">Value indicating whether the special case for encoding of final character spaces is used.</param>
        /// <returns>An encoded version of the input string suitable for use in LDAP DNs.</returns>
        internal static string DistinguishedNameEncode(string input, bool useInitialCharacterRules, bool useFinalCharacterRule)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (distinguishedNameCharacterValues == null)
            {
                InitialiseDistinguishedNameSafeList();
            }

            byte[] utf8Bytes = Encoding.UTF8.GetBytes(input.ToCharArray());
            char[] encodedInput = new char[utf8Bytes.Length * 3]; // Each byte can potentially be encoded as #xx
            int outputLength = 0;

            DistinguishedNameSafeListSyncLock.EnterReadLock();
            try
            {
                for (int characterPosition = 0; characterPosition < utf8Bytes.Length; characterPosition++)
                {
                    byte currentCharacter = utf8Bytes[characterPosition];

                    if (characterPosition == 0 && currentCharacter == ' ' && useInitialCharacterRules)
                    {
                        // rfc2253 states spaces at the start of a string must be escaped
                        encodedInput[outputLength++] = '\\';
                        encodedInput[outputLength++] = ' ';
                    }
                    else if (characterPosition == 0 && currentCharacter == '#' && useInitialCharacterRules)
                    {
                        // rfc2253 states hashes at the start of a string must be escaped
                        encodedInput[outputLength++] = '\\';
                        encodedInput[outputLength++] = '#';
                    }
                    else if (characterPosition == (utf8Bytes.Length - 1) && currentCharacter == ' ' &&
                             useFinalCharacterRule)
                    {
                        // rfc2253 states spaces at the end of a string must be escaped
                        encodedInput[outputLength++] = '\\';
                        encodedInput[outputLength++] = ' ';
                    }
                    else if (distinguishedNameCharacterValues[currentCharacter] != null)
                    {
                        // Character needs encoding.
                        char[] encodedCharacter = distinguishedNameCharacterValues[currentCharacter];

                        for (int j = 0; j < encodedCharacter.Length; j++)
                        {
                            encodedInput[outputLength++] = encodedCharacter[j];
                        }
                    }
                    else
                    {
                        // Character does not need encoding.
                        encodedInput[outputLength++] = (char)currentCharacter;
                    }
                }
            }
            finally
            {
                DistinguishedNameSafeListSyncLock.ExitReadLock();
            }

            return new string(encodedInput, 0, outputLength);
        }

        /// <summary>
        /// Initializes the LDAP filter safe lists.
        /// </summary>
        private static void InitialiseFilterSafeList()
        {
            FilterSafeListSyncLock.EnterWriteLock();
            try
            {
                if (filterCharacterValues == null)
                {
                    filterCharacterValues = SafeList.Generate(255, SafeList.SlashThenHexValueGenerator);
                    SafeList.PunchSafeList(ref filterCharacterValues, FilterEncodingSafeList());
                }
            }
            finally
            {
                FilterSafeListSyncLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Provides the safe characters for LDAP filter encoding.
        /// </summary>
        /// <returns>The safe characters for LDAP filter encoding.</returns>
        /// <remarks>See http://tools.ietf.org/html/rfc4515/</remarks>
        private static IEnumerable FilterEncodingSafeList()
        {
            for (int i = 0x20; i <= 0x7E; i++)
            {
                // Escape dangerous filter characters
                // See http://projects.webappsec.org/LDAP-Injection
                if (i == '(' ||
                    i == ')' ||
                    i == '*' ||
                    i == '/' ||
                    i == '\\')
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Initializes the LDAP DN safe lists.
        /// </summary>
        private static void InitialiseDistinguishedNameSafeList()
        {
            DistinguishedNameSafeListSyncLock.EnterWriteLock();
            try
            {
                if (distinguishedNameCharacterValues == null)
                {
                    distinguishedNameCharacterValues = SafeList.Generate(255, SafeList.HashThenHexValueGenerator);
                    SafeList.PunchSafeList(ref distinguishedNameCharacterValues, DistinguishedNameSafeList());

                    // Now mark up the specially listed characters from http://www.ietf.org/rfc/rfc2253.txt
                    EscapeDistinguisedNameCharacter(',');
                    EscapeDistinguisedNameCharacter('+');
                    EscapeDistinguisedNameCharacter('"');
                    EscapeDistinguisedNameCharacter('\\');
                    EscapeDistinguisedNameCharacter('<');
                    EscapeDistinguisedNameCharacter('>');
                    EscapeDistinguisedNameCharacter(';');
                }
            }
            finally
            {
                DistinguishedNameSafeListSyncLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Provides the safe characters for LDAP filter encoding.
        /// </summary>
        /// <returns>The safe characters for LDAP filter encoding.</returns>
        /// <remarks>See http://www.ietf.org/rfc/rfc2253.txt </remarks>
        private static IEnumerable DistinguishedNameSafeList()
        {
            for (int i = 0x20; i <= 0x7E; i++)
            {
                // RFC mandated escapes.
                if (i == ',' ||
                    i == '+' ||
                    i == '"' ||
                    i == '\\' ||
                    i == '<' ||
                    i == '>')
                {
                    continue;
                }

                // Safety escapes
                // See http://projects.webappsec.org/LDAP-Injection
                if (i == '&' ||
                    i == '!' ||
                    i == '|' ||
                    i == '=' ||
                    i == '-' ||
                    i == '\'' ||
                    i == ';')
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Escapes a special DN character.
        /// </summary>
        /// <param name="c">The character to escape.</param>
        private static void EscapeDistinguisedNameCharacter(char c)
        {
            distinguishedNameCharacterValues[c] = new[] { '\\', c };
        }
    }
}
