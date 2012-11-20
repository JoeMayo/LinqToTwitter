// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Middle.cs" company="Microsoft Corporation">
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
//   Provides safe character positions for the lower middle section of the UTF code tables.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LinqToTwitter.Security.Application.CodeCharts
{
    using System.Collections;

    /// <summary>
    /// Provides safe character positions for the middle section of the UTF code tables.
    /// </summary>
    internal static class Middle
    {
        /// <summary>
        /// Determines if the specified flag is set.
        /// </summary>
        /// <param name="flags">The value to check.</param>
        /// <param name="flagToCheck">The flag to check for.</param>
        /// <returns>true if the flag is set, otherwise false.</returns>
        public static bool IsFlagSet(MidCodeCharts flags, MidCodeCharts flagToCheck)
        {
            return (flags & flagToCheck) != 0;
        }

        /// <summary>
        /// Provides the safe characters for the Greek Extended code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable GreekExtended()
        {
            for (int i = 0x1F00; i <= 0x1FFE; i++)
            {
                if (i == 0x1F16 ||
                    i == 0x1F17 ||
                    i == 0x1F1E ||
                    i == 0x1F1F ||
                    i == 0x1F46 ||
                    i == 0x1F47 ||
                    i == 0x1F4E ||
                    i == 0x1F4F ||
                    i == 0x1F58 ||
                    i == 0x1F5A ||
                    i == 0x1F5C ||
                    i == 0x1F5E ||
                    i == 0x1F7E ||
                    i == 0x1F7F ||
                    i == 0x1FB5 ||
                    i == 0x1FC5 ||
                    i == 0x1FD4 ||
                    i == 0x1FD5 ||
                    i == 0x1FDC ||
                    i == 0x1FF0 ||
                    i == 0x1FF1 ||
                    i == 0x1FF5)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the General Punctuation code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable GeneralPunctuation()
        {
            for (int i = 0x2000; i <= 0x206F; i++)
            {
                if (i >= 0x2065 && i <= 0x2069)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Superscripts and subscripts code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable SuperscriptsAndSubscripts()
        {
            for (int i = 0x2070; i <= 0x2094; i++)
            {
                if (i == 0x2072 ||
                    i == 0x2073 ||
                    i == 0x208F)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Currency Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CurrencySymbols()
        {
            for (int i = 0x20A0; i <= 0x20B8; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Combining Diacritrical Marks for Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable CombiningDiacriticalMarksForSymbols()
        {
            for (int i = 0x20D0; i <= 0x20F0; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Letterlike Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable LetterlikeSymbols()
        {
            for (int i = 0x2100; i <= 0x214F; i++)
            {
                yield return i;
            }
        }
    
        /// <summary>
        /// Provides the safe characters for the Number Forms code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable NumberForms()
        {
            for (int i = 0x2150; i <= 0x2189; i++)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Arrows code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Arrows()
        {
            for (int i = 0x2190; i <= 0x21FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Mathematical Operators code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable MathematicalOperators()
        {
            for (int i = 0x2200; i <= 0x22FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Miscellaneous Technical code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable MiscellaneousTechnical()
        {
            for (int i = 0x2300; i <= 0x23E8; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Control Pictures code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable ControlPictures()
        {
            for (int i = 0x2400; i <= 0x2426; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the OCR code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable OpticalCharacterRecognition()
        {
            for (int i = 0x2440; i <= 0x244A; i++)
            {
                yield return i;
            }        
        }

        /// <summary>
        /// Provides the safe characters for the Enclosed Alphanumerics code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable EnclosedAlphanumerics()
        {
            for (int i = 0x2460; i <= 0x24FF; i++)
            {
                yield return i;
            }                
        }

        /// <summary>
        /// Provides the safe characters for the Box Drawing code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable BoxDrawing()
        {
            for (int i = 0x2500; i <= 0x257F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Block Elements code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable BlockElements()
        {
            for (int i = 0x2580; i <= 0x259F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Geometric Shapes code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable GeometricShapes()
        {
            for (int i = 0x25A0; i <= 0x25FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Miscellaneous Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable MiscellaneousSymbols()
        {
            for (int i = 0x2600; i <= 0x26FF; i++)
            {
                if (i == 0x26CE || 
                    i == 0x26E2 ||
                    (i >= 0x26E4 && i <= 0x26E7))
                {
                    continue;
                }

                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Dingbats code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Dingbats()
        {
            for (int i = 0x2701; i <= 0x27BE; i++)
            {
                if (i == 0x2705 ||
                    i == 0x270A ||
                    i == 0x270B ||
                    i == 0x2728 ||
                    i == 0x274C ||
                    i == 0x274E ||
                    i == 0x2753 ||
                    i == 0x2754 ||
                    i == 0x2755 ||
                    i == 0x275F ||
                    i == 0x2760 ||
                    i == 0x2795 ||
                    i == 0x2796 ||
                    i == 0x2797 ||
                    i == 0x27B0)
                {
                    continue;
                }

                yield return i;
            }
        }
        
        /// <summary>
        /// Provides the safe characters for the Miscellaneous Mathematical Symbols A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable MiscellaneousMathematicalSymbolsA()
        {
            for (int i = 0x27C0; i <= 0x27EF; i++)
            {
                if (i == 0x27CB ||
                    i == 0x27CD ||
                    i == 0x27CE ||
                    i == 0x27CF)
                {
                    continue;
                }

                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Supplemental Arrows A code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable SupplementalArrowsA()
        {
            for (int i = 0x27F0; i <= 0x27FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Braille Patterns code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable BraillePatterns()
        {
            for (int i = 0x2800; i <= 0x28FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Supplemental Arrows B code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable SupplementalArrowsB()
        {
            for (int i = 0x2900; i <= 0x297F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Miscellaneous Mathematical Symbols B code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable MiscellaneousMathematicalSymbolsB()
        {
            for (int i = 0x2980; i <= 0x29FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Supplemental Mathematical Operators code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable SupplementalMathematicalOperators()
        {
            for (int i = 0x2A00; i <= 0x2AFF; i++)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Miscellaneous Symbols and Arrows code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable MiscellaneousSymbolsAndArrows()
        {
            for (int i = 0x2B00; i <= 0x2B59; i++)
            {
                if (i == 0x2B4D || 
                    i == 0x2B4E ||
                    i == 0x2B4F)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Glagolitic code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Glagolitic()
        {
            for (int i = 0x2C00; i <= 0x2C5E; i++)
            {
                if (i == 0x2C2F)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Latin Extended C code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable LatinExtendedC()
        {
            for (int i = 0x2C60; i <= 0x2C7F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Coptic table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Coptic()
        {
            for (int i = 0x2C80; i <= 0x2CFF; i++)
            {
                if (i >= 0x2CF2 && i <= 0x2CF8)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Georgian Supplement code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable GeorgianSupplement()
        {
            for (int i = 0x2D00; i <= 0x2D25; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Tifinagh code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Tifinagh()
        {
            for (int i = 0x2D30; i <= 0x2D6F; i++)
            {
                if (i >= 0x2D66 && i <= 0x2D6E)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Ethiopic Extended code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable EthiopicExtended()
        {
            for (int i = 0x2D80; i <= 0x2DDE; i++)
            {
                if ((i >= 0x2D97 && i <= 0x2D9F) ||
                    i == 0x2DA7 ||
                    i == 0x2DAF ||
                    i == 0x2DB7 ||
                    i == 0x2DBF ||
                    i == 0x2DC7 ||
                    i == 0x2DCF ||
                    i == 0x2DD7 ||
                    i == 0x2DDF)
                {
                    continue;
                }

                yield return i;
            }
        }
    }
}
