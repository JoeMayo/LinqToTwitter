// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LowerMiddle.cs" company="Microsoft Corporation">
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

namespace Microsoft.Security.Application.CodeCharts
{
    using System.Collections;

    /// <summary>
    /// Provides safe character positions for the lower middle section of the UTF code tables.
    /// </summary>
    internal static class LowerMiddle
    {
        /// <summary>
        /// Determines if the specified flag is set.
        /// </summary>
        /// <param name="flags">The value to check.</param>
        /// <param name="flagToCheck">The flag to check for.</param>
        /// <returns>true if the flag is set, otherwise false.</returns>
        public static bool IsFlagSet(LowerMidCodeCharts flags, LowerMidCodeCharts flagToCheck)
        {
            return (flags & flagToCheck) != 0;
        }

        /// <summary>
        /// Provides the safe characters for the Myanmar code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>
        public static IEnumerable Myanmar()
        {
            for (int i = 0x1000; i <= 0x109F; i++)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Georgian code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>        
        public static IEnumerable Georgian()
        {
            for (int i = 0x10A0; i <= 0x10FC; i++)
            {
                if (i >= 0x10C6 && i <= 0x10CF)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Hangul Jamo code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable HangulJamo()
        {
            for (int i = 0x1100; i <= 0x11FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Ethiopic code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Ethiopic()
        {
            for (int i = 0x1200; i <= 0x137C; i++)
            {
                if (i == 0x1249 ||
                    i == 0x124E ||
                    i == 0x124F ||
                    i == 0x1257 ||
                    i == 0x1259 ||
                    i == 0x125E ||
                    i == 0x125F ||
                    i == 0x1289 ||
                    i == 0x128E ||
                    i == 0x128F ||
                    i == 0x12B1 ||
                    i == 0x12B6 ||
                    i == 0x12B7 ||
                    i == 0x12BF ||
                    i == 0x12C1 ||
                    i == 0x12C6 ||
                    i == 0x12C7 ||
                    i == 0x12D7 ||
                    i == 0x1311 ||
                    i == 0x1316 ||
                    i == 0x1317 ||
                    (i >= 0x135B && i <= 0x135E))
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Ethiopic Supplement code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable EthiopicSupplement()
        {
            for (int i = 0x1380; i <= 0x1399; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Cherokee code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Cherokee()
        {
            for (int i = 0x13A0; i <= 0x13F4; i++)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Unified Canadian Aboriginal Syllabic code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable UnifiedCanadianAboriginalSyllabics()
        {
            for (int i = 0x1400; i <= 0x167F; i++)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Ogham code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Ogham()
        {
            for (int i = 0x1680; i <= 0x169C; i++)
            {
                yield return i;
            }              
        }

        /// <summary>
        /// Provides the safe characters for the Runic code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Runic()
        {
            for (int i = 0x16A0; i <= 0x16F0; i++)
            {
                yield return i;
            }              
        }

        /// <summary>
        /// Provides the safe characters for the Tagalog code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Tagalog()
        {
            for (int i = 0x1700; i <= 0x1714; i++)
            {
                if (i == 0x170D)
                {
                    continue;
                }

                yield return i;
            }               
        }

        /// <summary>
        /// Provides the safe characters for the Hanunoo code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Hanunoo()
        {
            for (int i = 0x1720; i <= 0x1736; i++)
            {
                yield return i;
            }             
        }

        /// <summary>
        /// Provides the safe characters for the Buhid code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Buhid()
        {
            for (int i = 0x1740; i <= 0x1753; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Tagbanwa code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Tagbanwa()
        {
            for (int i = 0x1760; i <= 0x1773; i++)
            {
                if (i == 0x176D ||
                    i == 0x1771)
                {
                    continue;
                }

                yield return i;
            }                         
        }

        /// <summary>
        /// Provides the safe characters for the Khmer code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Khmer()
        {
            for (int i = 0x1780; i <= 0x17F9; i++)
            {
                if (i == 0x17DE ||
                    i == 0x17DF ||
                    (i >= 0x17EA && i <= 0x17EF))
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Mongolian code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns> 
        public static IEnumerable Mongolian()
        {
            for (int i = 0x1800; i <= 0x18AA; i++)
            {
                if (i == 0x180F ||
                    (i >= 0x181A && i <= 0x181F) ||
                    (i >= 0x1878 && i <= 0x187F))
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Unified Canadian Aboriginal Syllabic Extended code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable UnifiedCanadianAboriginalSyllabicsExtended()
        {
            for (int i = 0x18B0; i <= 0x18F5; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Limbu code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable Limbu()
        {
            for (int i = 0x1900; i <= 0x194F; i++)
            {
                if (i == 0x191D ||
                    i == 0x191E ||
                    i == 0x191F ||
                    (i >= 0x192C && i <= 0x192F) ||
                    (i >= 0x193C && i <= 0x193F) ||
                    i == 0x1941 ||
                    i == 0x1942 ||
                    i == 0x1943)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Tai Le code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable TaiLe()
        {
            for (int i = 0x1950; i <= 0x1974; i++)
            {
                if (i == 0x196E ||
                    i == 0x196F)
                {
                    continue;
                }
                
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the New Tai Lue code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>          
        public static IEnumerable NewTaiLue()
        {
            for (int i = 0x1980; i <= 0x19DF; i++)
            {
                if ((i >= 0x19AC && i <= 0x19AF) ||
                    (i >= 0x19CA && i <= 0x19CF) ||
                    (i >= 0x19DB && i <= 0x19DD))
                {
                    continue;
                }

                yield return i;            
            }
        }

        /// <summary>
        /// Provides the safe characters for the Khmer Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable KhmerSymbols()
        {
            for (int i = 0x19E0; i <= 0x19FF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Khmer Symbols code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Buginese()
        {
            for (int i = 0x1A00; i <= 0x1A1F; i++)
            {
                if (i == 0x1A1C ||
                    i == 0x1A1D)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Tai Tham code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable TaiTham()
        {
            for (int i = 0x1A20; i <= 0x1AAD; i++)
            {
                if (i == 0x1A5F ||
                    i == 0x1A7D ||
                    i == 0x1A7E ||
                    (i >= 0x1A8A && i <= 0x1A8F) ||
                    (i >= 0x1A9A && i <= 0x1A9F))
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Balinese code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Balinese()
        {
            for (int i = 0x1B00; i <= 0x1B7C; i++)
            {
                if (i >= 0x1B4C && i <= 0x1B4F)
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Sudanese code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Sudanese()
        {
            for (int i = 0x1B80; i <= 0x1BB9; i++)
            {
                if (i >= 0x1BAB && i <= 0x1BAD)
                {
                    continue;
                }

                yield return i;                
            }
        }

        /// <summary>
        /// Provides the safe characters for the Lepcha code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>   
        public static IEnumerable Lepcha()
        {
            for (int i = 0x1C00; i <= 0x1C4F; i++)
            {
                if ((i >= 0x1C38 && i <= 0x1C3A) ||
                    (i >= 0x1C4A && i <= 0x1C4C))
                {
                    continue;
                }

                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Ol Chiki code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable OlChiki()
        {
            for (int i = 0x1C50; i <= 0x1C7F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Vedic Extensions code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable VedicExtensions()
        {
            for (int i = 0x1CD0; i <= 0x1CF2; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Phonetic Extensions code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable PhoneticExtensions()
        {
            for (int i = 0x1D00; i <= 0x1D7F; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Phonetic Extensions Supplement code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable PhoneticExtensionsSupplement()
        {
            for (int i = 0x1D80; i <= 0x1DBF; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Provides the safe characters for the Combining Diacritical Marks Supplement code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable CombiningDiacriticalMarksSupplement()
        {
            for (int i = 0x1DC0; i <= 0x1DFF; i++)
            {
                if (i >= 0x1DE7 && i <= 0x1DFC)
                {
                    continue;
                }

                yield return i;
            }            
        }

        /// <summary>
        /// Provides the safe characters for the Latin Extended Addition code table.
        /// </summary>
        /// <returns>The safe characters for the code table.</returns>  
        public static IEnumerable LatinExtendedAdditional()
        {
            for (int i = 0x1E00; i <= 0x1EFF; i++)
            {
                yield return i;
            }
        }
    }
}
