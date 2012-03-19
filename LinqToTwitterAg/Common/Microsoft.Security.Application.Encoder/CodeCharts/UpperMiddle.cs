// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpperMiddle.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
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
//   Provides safe character positions for the upper middle section of the UTF code tables.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.CodeCharts
{
    using System.Collections;

    /// <summary>
    /// Provides safe character positions for the upper middle section of the UTF code tables.
    /// </summary>
    internal static class UpperMiddle
    {
        /// <summary>
        /// Determines if the specified flag is set.
        /// </summary>
        /// <param name="flags">The value to check.</param>
        /// <param name="flagToCheck">The flag to check for.</param>
        /// <returns>true if the flag is set, otherwise false.</returns>
        public static bool IsFlagSet(UpperMidCodeCharts flags, UpperMidCodeCharts flagToCheck)
        {
            return (flags & flagToCheck) != 0;
        }

        /// <summary>
        /// Provides the safe characters for the Cyrillic Extended A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CyrillicExtendedA()
        {
            for (int i = 0x2DE0; i <= 0x2DFF; i++)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Cyrillic Extended A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable SupplementalPunctuation()
        {
            for (int i = 0x2E00; i <= 0x2E31; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the CJK Radicals Supplement code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CjkRadicalsSupplement()
        {
            for (int i = 0x2E80; i <= 0x2EF3; i++)
            {
                if (i == 0x2E9A)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Kangxi Radicals code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable KangxiRadicals()
        {
            for (int i = 0x2F00; i <= 0x2FD5; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Ideographic Description Characters code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable IdeographicDescriptionCharacters()
        {
            for (int i = 0x2FF0; i <= 0x2FFB; i++)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the CJK Symbols and Punctuation code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CjkSymbolsAndPunctuation()
        {
            for (int i = 0x3000; i <= 0x303F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Hiragana code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Hiragana()
        {
            for (int i = 0x3041; i <= 0x309F; i++)
            {
                if (i == 0x3097 ||
                    i == 0x3098)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Hiragana code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Katakana()
        {
            for (int i = 0x30A0; i <= 0x30FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Bopomofo code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Bopomofo()
        {
            for (int i = 0x3105; i <= 0x312D; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Hangul Compatibility Jamo code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable HangulCompatibilityJamo()
        {
            for (int i = 0x3131; i <= 0x318E; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Kanbun code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Kanbun()
        {
            for (int i = 0x3190; i <= 0x319F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Bopomofo Extended code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable BopomofoExtended()
        {
            for (int i = 0x31A0; i <= 0x31B7; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the CJK Strokes code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CjkStrokes()
        {
            for (int i = 0x31C0; i <= 0x31E3; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Katakana Phonetic Extensions code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable KatakanaPhoneticExtensions()
        {
            for (int i = 0x31F0; i <= 0x31FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Enclosed CJK Letters and Months code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable EnclosedCjkLettersAndMonths()
        {
            for (int i = 0x3200; i <= 0x32FE; i++)
            {
                if (i == 0x321F)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the CJK Compatibility code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CjkCompatibility()
        {
            for (int i = 0x3300; i <= 0x33FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the CJK Unified Ideographs Extension A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CjkUnifiedIdeographsExtensionA()
        {
            for (int i = 0x3400; i <= 0x4DB5; i++)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Yijing Hexagram Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable YijingHexagramSymbols()
        {
            for (int i = 0x4DC0; i <= 0x4DFF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the CJK Unified Ideographs code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CjkUnifiedIdeographs()
        {
            for (int i = 0x4E00; i <= 0x9FCB; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Yi Syllables code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable YiSyllables()
        {
            for (int i = 0xA000; i <= 0xA48C; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Yi Radicals code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable YiRadicals()
        {
            for (int i = 0xA490; i <= 0xA4C6; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Lisu code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Lisu()
        {
            for (int i = 0xA4D0; i <= 0xA4FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Vai code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Vai()
        {
            for (int i = 0xA500; i <= 0xA62B; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Cyrillic Extended B code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CyrillicExtendedB()
        {
            for (int i = 0xA640; i <= 0xA697; i++)
            {
                if (i == 0xA660 ||
                    i == 0xA661 ||
                    (i >= 0xA674 && i <= 0xA67b))
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Bamum code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Bamum()
        {
            for (int i = 0xA6A0; i <= 0xA6F7; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Modifier Tone Letters code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable ModifierToneLetters()
        {
            for (int i = 0xA700; i <= 0xA71F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Latin Extended D code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable LatinExtendedD()
        {
            for (int i = 0xA720; i <= 0xA78C; i++)
            {
                yield return i;
            }

            for (int i = 0xA7FB; i <= 0xA7FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Syloti Nagri code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable SylotiNagri()
        {
            for (int i = 0xA800; i <= 0xA82B; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Common Indic Number Forms code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CommonIndicNumberForms()
        {
            for (int i = 0xA830; i <= 0xA839; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Phags-pa code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Phagspa()
        {
            for (int i = 0xA840; i <= 0xA877; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Saurashtra code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Saurashtra()
        {
            for (int i = 0xA880; i <= 0xA8D9; i++)
            {
                if (i >= 0xA8C5 && i <= 0xA8CD)
                {
                    continue;
                }

                yield return i;
            }
        }
    }
}
