// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Upper.cs" company="Microsoft Corporation">
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
//   Provides safe character positions for the upper section of the UTF code tables.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application.CodeCharts
{
    using System.Collections;

    /// <summary>
    /// Provides safe character positions for the upper section of the UTF code tables.
    /// </summary>
    internal static class Upper
    {
        /// <summary>
        /// Determines if the specified flag is set.
        /// </summary>
        /// <param name="flags">The value to check.</param>
        /// <param name="flagToCheck">The flag to check for.</param>
        /// <returns>true if the flag is set, otherwise false.</returns>
        public static bool IsFlagSet(UpperCodeCharts flags, UpperCodeCharts flagToCheck)
        {
            return (flags & flagToCheck) != 0;
        }

        /// <summary>
        /// Provides the safe characters for the Devanagari Extended code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable DevanagariExtended()
        {
            for (int i = 0xA8E0; i <= 0xA8FB; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Kayah Li code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable KayahLi()
        {
            for (int i = 0xA900; i <= 0xA92F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Rejang code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Rejang()
        {
            for (int i = 0xA930; i <= 0xA953; i++)
            {
                yield return i;
            }

            yield return 0xA95F;
        }

        /// <summary>
        /// Provides the safe characters for the Hangul Jamo Extended A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable HangulJamoExtendedA()
        {
            for (int i = 0xA960; i <= 0xA97C; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Javanese code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Javanese()
        {
            for (int i = 0xA980; i <= 0xA9DF; i++)
            {
                if (i == 0xA9CE ||
                    (i >= 0xA9DA && i <= 0xA9DD))
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Cham code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Cham()
        {
            for (int i = 0xAA00; i <= 0xAA5F; i++)
            {
                if ((i >= 0xAA37 && i <= 0xAA3F) ||
                    i == 0xAA4E ||
                    i == 0xAA4F ||
                    i == 0xAA5A ||
                    i == 0xAA5B)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Myanmar Extended A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable MyanmarExtendedA()
        {
            for (int i = 0xAA60; i <= 0xAA7B; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Myanmar Extended A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable TaiViet()
        {
            for (int i = 0xAA80; i <= 0xAAC2; i++)
            {
                yield return i;
            }

            for (int i = 0xAADB; i <= 0xAADF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Meetei Mayek code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable MeeteiMayek()
        {
            for (int i = 0xABC0; i <= 0xABF9; i++)
            {
                if (i == 0xABEE ||
                    i == 0xABEF)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Hangul Syllables code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable HangulSyllables()
        {
            for (int i = 0xAC00; i <= 0xD7A3; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Hangul Jamo Extended B code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable HangulJamoExtendedB()
        {
            for (int i = 0xD7B0; i <= 0xD7FB; i++)
            {
                if (i == 0xD7C7 ||
                    i == 0xD7C8 ||
                    i == 0xD7C9 ||
                    i == 0xD7CA)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the CJK Compatibility Ideographs code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CjkCompatibilityIdeographs()
        {
            for (int i = 0xF900; i <= 0xFAD9; i++)
            {
                if (i == 0xFA2E ||
                    i == 0xFA2F ||
                    i == 0xFA6E ||
                    i == 0xFA6F)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Alphabetic Presentation Forms code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable AlphabeticPresentationForms()
        {
            for (int i = 0xFB00; i <= 0xFB4F; i++)
            {
                if ((i >= 0xFB07 && i <= 0xFB12) ||
                    (i >= 0xFB18 && i <= 0xFB1C) ||
                    i == 0xFB37 ||
                    i == 0xFB3D ||
                    i == 0xFB3F ||
                    i == 0xFB42 ||
                    i == 0xFB45)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Arabic Presentation Forms A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable ArabicPresentationFormsA()
        {
            for (int i = 0xFB50; i <= 0xFDFD; i++)
            {
                if ((i >= 0xFBB2 && i <= 0xFBD2) || 
                    (i >= 0xFD40 && i <= 0xFD4F) ||
                    i == 0xFD90 ||
                    i == 0xFD91 ||
                    (i >= 0xFDC8 && i <= 0xFDEF))
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Variation Selectors code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable VariationSelectors()
        {
            for (int i = 0xFE00; i <= 0xFE0F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Vertical Forms code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable VerticalForms()
        {
            for (int i = 0xFE10; i <= 0xFE19; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Combining Half Marks code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CombiningHalfMarks()
        {
            for (int i = 0xFE20; i <= 0xFE26; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the CJK Compatibility Forms code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CjkCompatibilityForms()
        {
            for (int i = 0xFE30; i <= 0xFE4F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Small Form Variants code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable SmallFormVariants()
        {
            for (int i = 0xFE50; i <= 0xFE6B; i++)
            {
                if (i == 0xFE53 || i == 0xFE67)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Arabic Presentation Forms B code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable ArabicPresentationFormsB()
        {
            for (int i = 0xFE70; i <= 0xFEFC; i++)
            {
                if (i == 0xFE75)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Half Width and Full Width Forms code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable HalfWidthAndFullWidthForms()
        {
            for (int i = 0xFF01; i <= 0xFFEE; i++)
            {
                if (i == 0xFFBF ||
                    i == 0xFFC0 ||
                    i == 0xFFC1 ||
                    i == 0xFFC8 ||
                    i == 0xFFC9 ||
                    i == 0xFFD0 ||
                    i == 0xFFD1 ||
                    i == 0xFFD8 ||
                    i == 0xFFD9 ||
                    i == 0xFFDD ||
                    i == 0xFFDE ||
                    i == 0xFFDF ||
                    i == 0xFFE7)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Specials code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Specials()
        {
            for (int i = 0xFFF9; i <= 0xFFFD; i++)
            {
                yield return i;
            }
        }
    }
}
