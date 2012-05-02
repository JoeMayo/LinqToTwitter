// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CssEncoder.cs" company="Microsoft Corporation">
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
//   Provides CSS Encoding methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using LinqToTwitter.Common;

namespace Microsoft.Security.Application
{
    using System.Collections;

    /// <summary>
    /// Provides CSS Encoding methods.
    /// </summary>
    internal static class CssEncoder
    {
        /// <summary>
        /// A lock object to use when performing safe listing.
        /// </summary>
        private static readonly ReaderWriterLock syncLock = new ReaderWriterLock();

        /// <summary>
        /// The values to output for each character.
        /// </summary>
        private static char[][] characterValues;

        /// <summary>
        /// Encodes according to the CSS encoding rules.
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        /// <exception cref="InvalidUnicodeValueException">Thrown if a character with an invalid Unicode value is encountered within the input string.</exception>
        /// <exception cref="InvalidSurrogatePairException">Thrown if a high surrogate code point is encoded without a following low surrogate code point, or a 
        /// low surrogate code point is encounter without having been preceded by a high surrogate code point.</exception>
        internal static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (characterValues == null)
            {
                InitialiseSafeList();
            }

            // Setup a new character array for output.
            char[] inputAsArray = input.ToCharArray();
            int outputLength = 0;
            int inputLength = inputAsArray.Length;
            char[] encodedInput = new char[inputLength * 7]; // Worse case scenario - CSS encoding wants \XXXXXX for encoded characters.

            syncLock.EnterReadLock();
            try
            {
                for (int i = 0; i < inputLength; i++)
                {
                    char currentCharacter = inputAsArray[i];
                    int currentCodePoint = inputAsArray[i];

                    // Check for invalid values
                    if (currentCodePoint == 0xFFFE ||
                        currentCodePoint == 0xFFFF)
                    {
                        throw new InvalidUnicodeValueException(currentCodePoint);
                    }
                    else if (currentCharacter.IsHighSurrogate())
                    {
                        if (i + 1 == inputLength)
                        {
                            throw new InvalidSurrogatePairException(currentCharacter, '\0');
                        }

                        // Now peak ahead and check if the following character is a low surrogate.
                        char nextCharacter = inputAsArray[i + 1];
                        char nextCodePoint = inputAsArray[i + 1];
                        if (!nextCharacter.IsLowSurrogate())
                        {
                            throw new InvalidSurrogatePairException(currentCharacter, nextCharacter);
                        }

                        // Look-ahead was good, so skip.
                        i++;

                        // Calculate the combined code point
                        long combinedCodePoint =
                            0x10000 + ((currentCodePoint - 0xD800) * 0x400) + (nextCodePoint - 0xDC00);
                        char[] encodedCharacter = SafeList.SlashThenSixDigitHexValueGenerator(combinedCodePoint);

                        for (int j = 0; j < encodedCharacter.Length; j++)
                        {
                            encodedInput[outputLength++] = encodedCharacter[j];
                        }
                    }
                    else if (currentCharacter.IsLowSurrogate())
                    {
                        throw new InvalidSurrogatePairException('\0', currentCharacter);
                    }
                    else if (currentCodePoint > characterValues.Length - 1)
                    {
                        char[] encodedCharacter = SafeList.SlashThenSixDigitHexValueGenerator(currentCodePoint);

                        for (int j = 0; j < encodedCharacter.Length; j++)
                        {
                            encodedInput[outputLength++] = encodedCharacter[j];
                        }
                    }
                    else if (characterValues[currentCodePoint] != null)
                    {
                        // character needs to be encoded
                        char[] encodedCharacter = characterValues[currentCodePoint];
                        for (int j = 0; j < encodedCharacter.Length; j++)
                        {
                            encodedInput[outputLength++] = encodedCharacter[j];
                        }
                    }
                    else
                    {
                        // character does not need encoding
                        encodedInput[outputLength++] = currentCharacter;
                    }
                }
            }
            finally
            {
                syncLock.ExitReadLock();
            }

            return new string(encodedInput, 0, outputLength);
        }

        /// <summary>
        /// Initializes the HTML safe list.
        /// </summary>
        private static void InitialiseSafeList()
        {
            syncLock.EnterWriteLock();
            try
            {
                if (characterValues == null)
                {
                    characterValues = SafeList.Generate(0xFF, SafeList.SlashThenSixDigitHexValueGenerator);
                    SafeList.PunchSafeList(ref characterValues, CssSafeList());
                }
            }
            finally
            {
                syncLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Provides the safe characters for CS encoding.
        /// </summary>
        /// <returns>The safe characters for CSS encoding.</returns>
        /// <remarks>See http://www.owasp.org/index.php/XSS_(Cross_Site_Scripting)_Prevention_Cheat_Sheet </remarks>
        private static IEnumerable CssSafeList()
        {
            for (int i = '0'; i <= '9'; i++)
            {
                yield return i;
            }

            for (int i = 'A'; i <= 'Z'; i++)
            {
                yield return i;
            }

            for (int i = 'a'; i <= 'z'; i++)
            {
                yield return i;
            }

            // Extended higher ASCII, Ç to É
            for (int i = 0x80; i <= 0x90; i++)
            {
                yield return i;
            }

            // Extended higher ASCII, ô to Ü
            for (int i = 0x93; i <= 0x9A; i++)
            {
                yield return i;
            }

            // Extended higher ASCII, á to Ñ
            for (int i = 0xA0; i <= 0xA5; i++)
            {
                yield return i;
            }
        }
    }
}
