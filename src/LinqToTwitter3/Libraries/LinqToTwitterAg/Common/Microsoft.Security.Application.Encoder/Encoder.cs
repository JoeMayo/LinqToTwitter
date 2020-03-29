// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Encoder.cs" company="Microsoft Corporation">
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
//   Performs encoding of input strings to provide protection against
//   Cross-Site Scripting (XSS) attacks and LDAP injection attacks in
//   various contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LinqToTwitter.Security.Application
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Performs encoding of input strings to provide protection against
    /// Cross-Site Scripting (XSS) attacks and LDAP injection attacks in 
    /// various contexts.
    /// </summary>
    /// <remarks>
    /// This encoding library uses the Principle of Inclusions, 
    /// sometimes referred to as "safe-listing" to provide protection 
    /// against injection attacks.  With safe-listing protection, 
    /// algorithms look for valid inputs and automatically treat 
    /// everything outside that set as a potential attack.  This library 
    /// can be used as a defense in depth approach with other mitigation 
    /// techniques. It is suitable for applications with high security 
    /// requirements.
    /// </remarks>
    public static class Encoder
    {
        /// <summary>
        /// Empty string for Visual Basic Script context
        /// </summary>
        private const string VbScriptEmptyString = "\"\"";

        /// <summary>
        /// Empty string for Java Script context
        /// </summary>
        private const string JavaScriptEmptyString = "''";

        /// <summary>
        /// Initializes character Html encoding array
        /// </summary>
        private static readonly char[][] safeListCodes = InitializeSafeList();

        /// <summary>
        /// Encodes input strings for use as a value  in Lightweight Directory Access Protocol (LDAP) filter queries.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>Encoded string for use as a value in LDAP filter queries.</returns>
        /// <remarks>This method encodes all but known safe characters defined in the safe list.
        /// <newpara/>
        /// RFC 4515 defines the format in which special characters need to be 
        /// escaped to be used inside a search filter. Special characters need to be 
        /// encoded in \XX format where XX is the hex representation of the character.
        /// <newpara/>
        /// The following examples illustrate the use of the escaping mechanism.
        /// <list type="table">
        /// <item><term>Parens R Us (for all your parenthetical needs)</term><description>Parens R Us \28for all your parenthetical needs\29</description></item>
        /// <item><term>*</term><description>\2A</description></item>
        /// <item><term>C:\MyFile</term><description>C:\5CMyFile</description></item>
        /// <item><term>NULLNULLNULLEOT (binary)</term><description>\00\00\00\04</description></item>
        /// <item><term>Lučić</term><description>Lu\C4\8Di\C4\87</description></item>
        /// </list>
        /// </remarks>
        public static string LdapFilterEncode(string input)
        {
            return LdapEncoder.FilterEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use as a value in Lightweight Directory Access Protocol (LDAP) DNs.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>Encoded string for use as a value in LDAP DNs.</returns>
        /// <remarks>This method encodes all but known safe characters defined in the safe list.
        /// <newpara/>
        /// RFC 2253 defines the format in which special characters need to be 
        /// escaped to be used inside a search filter. Special characters need to be 
        /// encoded in #XX format where XX is the hex representation of the character or a 
        /// specific \ escape format.
        /// <newpara/>
        /// The following examples illustrate the use of the escaping mechanism.
        /// <list type="table">
        /// <item><term>, + \ " \ &lt; &gt;</term><description>\, \+ \" \\ \&lt; \&gt;</description></item>
        /// <item><term> hello</term><description>\ hello</description></item>
        /// <item><term>hello </term><description>hello \ </description></item>
        /// <item><term>#hello</term><description>\#hello</description></item>
        /// <item><term>Lučić</term><description>Lu#C4#8Di#C4#87</description></item>
        /// </list>
        /// </remarks>
        public static string LdapDistinguishedNameEncode(string input)
        {
            return LdapDistinguishedNameEncode(input, true, true);
        }

        /// <summary>
        /// Encodes input strings for use as a value in Lightweight Directory Access Protocol (LDAP) DNs.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="useInitialCharacterRules">Value indicating whether the special case rules for encoding of spaces and octothorpes at the start of a string are used.</param>
        /// <param name="useFinalCharacterRule">Value indicating whether the special case for encoding of final character spaces is used.</param>
        /// <returns>Encoded string for use as a value in LDAP DNs.</returns>\
        /// <remarks>This method encodes all but known safe characters defined in the safe list.
        /// <newpara/>
        /// RFC 2253 defines the format in which special characters need to be 
        /// escaped to be used inside a search filter. Special characters need to be 
        /// encoded in #XX format where XX is the hex representation of the character or a 
        /// specific \ escape format.
        /// <newpara/>
        /// The following examples illustrate the use of the escaping mechanism.
        /// <list type="table">
        /// <item><term>, + \ " \ &lt; &gt;</term><description>\, \+ \" \\ \&lt; \&gt;</description></item>
        /// <item><term> hello</term><description>\ hello</description></item>
        /// <item><term>hello </term><description>hello\ </description></item>
        /// <item><term>#hello</term><description>\#hello</description></item>
        /// <item><term>Lučić</term><description>Lu#C4#8Di#C4#87</description></item>
        /// </list>
        /// If useInitialCharacterRules is set to false then escaping of the initial space or octothorpe characters is not performed;
        /// <list type="table">
        /// <item><term>, + \ " \ &lt; &gt;</term><description>\, \+ \" \\ \&lt; \&gt;</description></item>
        /// <item><term> hello</term><description> hello</description></item>
        /// <item><term>hello </term><description>hello\ </description></item>
        /// <item><term>#hello</term><description>#hello</description></item>
        /// <item><term>Lučić</term><description>Lu#C4#8Di#C4#87</description></item>
        /// </list>
        /// If useFinalCharacterRule is set to false then escaping of a space at the end of a string is not performed;
        /// <list type="table">
        /// <item><term>, + \ " \ &lt; &gt;</term><description>\, \+ \" \\ \&lt; \&gt;</description></item>
        /// <item><term> hello</term><description> hello</description></item>
        /// <item><term>hello </term><description>hello </description></item>
        /// <item><term>#hello</term><description>#hello</description></item>
        /// <item><term>Lučić</term><description>Lu#C4#8Di#C4#87</description></item>
        /// </list>
        /// </remarks>
        public static string LdapDistinguishedNameEncode(string input, bool useInitialCharacterRules, bool useFinalCharacterRule)
        {
            return LdapEncoder.DistinguishedNameEncode(input, useInitialCharacterRules, useFinalCharacterRule);
        }

        /// <summary>
        /// Encodes input strings to be used as a value in Lightweight Directory Access Protocol (LDAP) search queries.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>Encoded string for use in LDAP search queries.</returns>
        /// <remarks>This method encodes all but known safe characters defined in the safe list.
        /// <newpara/>
        /// RFC 4515 defines the format in which special characters need to be 
        /// escaped to be used inside a search filter. Special characters need to be 
        /// encoded in \XX format where XX is the hex representation of the character.
        /// <newpara/>
        /// The following examples illustrate the use of the escaping mechanism.
        /// <list type="table">
        /// <item><term>Parens R Us (for all your parenthetical needs)</term><description>Parens R Us \28for all your parenthetical needs\29</description></item>
        /// <item><term>*</term><description>\2A</description></item>
        /// <item><term>C:\MyFile</term><description>C:\5CMyFile</description></item>
        /// <item><term>NULLNULLNULLEOT (binary)</term><description>\00\00\00\04</description></item>
        /// <item><term>Lučić</term><description>Lu\C4\8Di\C4\87</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.LdapFilterEncode() instead.")]
        public static string LdapEncode(string input)
        {
            return LdapFilterEncode(input);
        }

        /// <summary>
        /// Encodes input strings used in Cascading Style Sheet (CSS) elements values.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>Encoded string for use in CSS element values.</returns>
        /// <remarks>This method encodes all but known safe characters defined in the safe list.
        /// <newpara/>
        /// The CSS character escape sequence consists of a backslash character (\) followed by 
        /// between one and six hexadecimal digits that represent a character code from the 
        /// ISO 10646 standard (which is equivalent to Unicode, for all intents and purposes). Any 
        /// character other than a hexadecimal digit will terminate the escape sequence. If a 
        /// character following the escape sequence is also a valid hexadecimal digit then it must 
        /// either include six digits in the escape, or use a whitespace character to terminate the 
        /// escape. This encoder enforces the six digit rule.
        /// For example \000020 denotes a space.
        /// </remarks>
        /// <exception cref="InvalidUnicodeValueException">Thrown if a character with an invalid Unicode value is encountered within the input string.</exception>
        /// <exception cref="InvalidSurrogatePairException">Thrown if a high surrogate code point is encoded without a following low surrogate code point, or a 
        /// low surrogate code point is encounter without having been preceded by a high surrogate code point.</exception>
        //public static string CssEncode(string input)
        //{
        //    return CssEncoder.Encode(input);
        //}

        /// <summary>
        /// Encodes input strings for use in HTML.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in HTML.
        /// </returns>
        /// <remarks>
        /// All characters not safe listed are encoded to their Unicode decimal value, using &amp;#DECIMAL; notation.
        /// The default safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>'</term><description>Apostrophe</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="UnicodeCharacterEncoder.MarkAsSafe"/>.
        /// <newpara/>
        /// Example inputs and their related encoded outputs:
        /// <list type="table">
        /// <item><term>&lt;script&gt;alert('XSS Attack!');&lt;/script&gt;</term><description>&amp;lt;script&amp;gt;alert('XSS Attack!');&amp;lt;/script&amp;gt;</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross Site Scripting Library</description></item>
        /// <item><term>"Anti-Cross Site Scripting Library"</term><description>&amp;quote;Anti-Cross Site Scripting Library&amp;quote;</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidUnicodeValueException">Thrown if a character with an invalid Unicode value is encountered within the input string.</exception>
        /// <exception cref="InvalidSurrogatePairException">Thrown if a high surrogate code point is encoded without a following low surrogate code point, or a 
        /// low surrogate code point is encounter without having been preceded by a high surrogate code point.</exception>
        public static string HtmlEncode(string input)
        {
            return HtmlEncode(input, false);
        }

        /// <summary>
        /// Encodes input strings for use in HTML.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="useNamedEntities">Value indicating if the HTML 4.0 named entities should be used.</param>
        /// <returns>
        /// Encoded string for use in HTML.
        /// </returns>
        /// <remarks>
        /// All characters not safe listed are encoded to their Unicode decimal value, using &amp;#DECIMAL; notation.
        /// If you choose to use named entities then if a character is an HTML4.0 named entity the named entity will be used.
        /// The default safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>'</term><description>Apostrophe</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="UnicodeCharacterEncoder.MarkAsSafe"/>.
        /// <newpara/>
        /// Example inputs and their related encoded outputs:
        /// <list type="table">
        /// <item><term>&lt;script&gt;alert('XSS Attack!');&lt;/script&gt;</term><description>&amp;lt;script&amp;gt;alert('XSS Attack!');&amp;lt;/script&amp;gt;</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross Site Scripting Library</description></item>
        /// <item><term>"Anti-Cross Site Scripting Library"</term><description>&amp;quote;Anti-Cross Site Scripting Library&amp;quote;</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidUnicodeValueException">Thrown if a character with an invalid Unicode value is encountered within the input string.</exception>
        /// <exception cref="InvalidSurrogatePairException">Thrown if a high surrogate code point is encoded without a following low surrogate code point, or a 
        /// low surrogate code point is encounter without having been preceded by a high surrogate code point.</exception>
        public static string HtmlEncode(string input, bool useNamedEntities)
        {
            return UnicodeCharacterEncoder.HtmlEncode(input, useNamedEntities);
        }

        /// <summary>
        /// Encodes an input string for use in an HTML attribute.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>The input string encoded for use in an HTML attribute.</returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using  &amp;#DECIMAL; notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="UnicodeCharacterEncoder.MarkAsSafe"/>.
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert(&amp;#39;XSS&amp;#32;Attack!&amp;#39;);</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross&amp;#32;Site&amp;#32;Scripting&amp;#32;Library</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidUnicodeValueException">Thrown if a character with an invalid Unicode value is encountered within the input string.</exception>
        /// <exception cref="InvalidSurrogatePairException">Thrown if a high surrogate code point is encoded without a following low surrogate code point, or a 
        /// low surrogate code point is encounter without having been preceded by a high surrogate code point.</exception>
        public static string HtmlAttributeEncode(string input)
        {
            return UnicodeCharacterEncoder.HtmlAttributeEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in universal resource locators (URLs).
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX 
        /// and %DOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert%28%27XSS%20Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross%20Site%20Scripting%20Library</description></item>
        /// </list>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "As this is meant as a replacement for HttpUility.Encode we must keep the same return type.")]
        public static string UrlEncode(string input)
        {
            return UrlEncode(input, Encoding.UTF8);
        }

        /// <summary>
        /// Encodes input strings for use in application/x-www-form-urlencoded form submissions.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX 
        /// and %DOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert%28%27XSS+Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross+Site+Scripting+Library</description></item>
        /// </list>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "This does not return a URL so the return type can be a string.")]
        public static string HtmlFormUrlEncode(string input)
        {
            return HtmlFormUrlEncode(input, Encoding.UTF8);
        }

        /// <summary>
        /// Encodes input strings for use in universal resource locators (URLs).
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="codePage">Codepage number of the input.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross%20Site%20Scripting%20Library</description></item>
        /// </list>
        /// </remarks>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage(
        //    "Microsoft.Design",
        //    "CA1055:UriReturnValuesShouldNotBeStrings",
        //    Justification = "This does not return a URL so the return type can be a string.")]
        //public static string UrlEncode(string input, int codePage)
        //{
        //    return UrlEncode(input, Encoding.GetEncoding(codePage));
        //}

        /// <summary>
        /// Encodes input strings for use in application/x-www-form-urlencoded form submissions.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="codePage">Codepage number of the input.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross+Site+Scripting+Library</description></item>
        /// </list>
        /// </remarks>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage(
        //    "Microsoft.Design",
        //    "CA1055:UriReturnValuesShouldNotBeStrings",
        //    Justification = "This not not return a URL, so the return type can be a string.")]
        //public static string HtmlFormUrlEncode(string input, int codePage)
        //{
        //    return HtmlFormUrlEncode(input, Encoding.GetEncoding(codePage));
        //}

        /// <summary>
        /// Encodes input strings for use in universal resource locators (URLs).
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="inputEncoding">Input encoding type.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// If the inputEncoding is null then UTF-8 is assumed by default.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross%20Site%20Scripting%20Library</description></item>
        /// </list>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "This does not return a URL so the return type can be a string.")]
        public static string UrlEncode(string input, Encoding inputEncoding)
        {
            // Assuming the default to be UTF-8
            if (inputEncoding == null)
            {
                inputEncoding = Encoding.UTF8;
            }

            return HtmlParameterEncoder.QueryStringParameterEncode(input, inputEncoding);
        }

        /// <summary>
        /// Encodes input strings for use in application/x-www-form-urlencoded form submissions.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="inputEncoding">Input encoding type.</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// If the inputEncoding is null then UTF-8 is assumed by default.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term>~</term><description>Tilde</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross+Site+Scripting+Library</description></item>
        /// </list>
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1055:UriReturnValuesShouldNotBeStrings",
            Justification = "This does not return a URL so the return type can be a string.")]
        public static string HtmlFormUrlEncode(string input, Encoding inputEncoding)
        {
            // Assuming the default to be UTF-8
            if (inputEncoding == null)
            {
                inputEncoding = Encoding.UTF8;
            }

            return HtmlParameterEncoder.FormStringParameterEncode(input, inputEncoding);
        }

        /// <summary>
        /// Encodes input strings for use in XML.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in XML.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters. Characters are encoded using &amp;#DECIMAL; notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="UnicodeCharacterEncoder.MarkAsSafe"/>.
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert(&amp;apos;XSS Attack!&amp;apos;);</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross Site Scripting Library</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidUnicodeValueException">Thrown if a character with an invalid Unicode value is encountered within the input string.</exception>
        /// <exception cref="InvalidSurrogatePairException">Thrown if a high surrogate code point is encoded without a following low surrogate code point, or a 
        /// low surrogate code point is encounter without having been preceded by a high surrogate code point.</exception>
        public static string XmlEncode(string input)
        {
            return UnicodeCharacterEncoder.XmlEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in XML attributes.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in XML attributes.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using &amp;#DECIMAL; notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// </list>
        /// The safe list may be adjusted using <see cref="UnicodeCharacterEncoder.MarkAsSafe"/>.
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert(&amp;apos;XSS&#32;Attack!&amp;apos);</description></item>
        /// <item><term>user@contoso.com</term><description>user@contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross&amp;#32;Site&amp;#32;Scripting&amp;#32;Library</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidUnicodeValueException">Thrown if a character with an invalid Unicode value is encountered within the input string.</exception>
        /// <exception cref="InvalidSurrogatePairException">Thrown if a high surrogate code point is encoded without a following low surrogate code point, or a 
        /// low surrogate code point is encounter without having been preceded by a high surrogate code point.</exception>
        public static string XmlAttributeEncode(string input)
        {
            // HtmlEncodeAttribute will handle input
            return UnicodeCharacterEncoder.XmlAttributeEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in JavaScript.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in JavaScript.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using \xSINGLE_BYTE_HEX and \uDOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>'alert\x28\x27XSS Attack\x21\x27\x29\x3b'</description></item>
        /// <item><term>user@contoso.com</term><description>'user\x40contoso.com'</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>'Anti-Cross Site Scripting Library'</description></item>
        /// </list>
        /// </remarks>
        public static string JavaScriptEncode(string input)
        {
            return JavaScriptEncode(input, true);
        }

        /// <summary>
        /// Encodes input strings for use in JavaScript.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <param name="emitQuotes">value indicating whether or not to emit quotes. true = emit quote. false = no quote.</param>
        /// <returns>
        /// Encoded string for use in JavaScript and does not return the output with en quotes.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using \xSINGLE_BYTE_HEX and \uDOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>'alert\x28\x27XSS Attack\x21\x27\x29\x3b'</description></item>
        /// <item><term>user@contoso.com</term><description>'user\x40contoso.com'</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>'Anti-Cross Site Scripting Library'</description></item>
        /// </list>
        /// </remarks>
        public static string JavaScriptEncode(string input, bool emitQuotes)
        {
            // Input validation: empty or null string condition
            if (string.IsNullOrEmpty(input))
            {
                return emitQuotes ? JavaScriptEmptyString : string.Empty;
            }

            // Use a new char array.
            int outputLength = 0;
            int inputLength = input.Length;
            char[] returnMe = new char[inputLength * 8]; // worst case length scenario

            // First step is to start the encoding with an apostrophe if flag is true.
            if (emitQuotes)
            {
                returnMe[outputLength++] = '\'';
            }

            for (int i = 0; i < inputLength; i++)
            {
                int currentCharacterAsInteger = input[i];
                char currentCharacter = input[i];
                if (safeListCodes[currentCharacterAsInteger] != null || currentCharacterAsInteger == 92 || (currentCharacterAsInteger >= 123 && currentCharacterAsInteger <= 127))
                {
                    // character needs to be encoded
                    if (currentCharacterAsInteger >= 127)
                    {
                        returnMe[outputLength++] = '\\';
                        returnMe[outputLength++] = 'u';
                        string hex = ((int)currentCharacter).ToString("x", CultureInfo.InvariantCulture).PadLeft(4, '0');
                        returnMe[outputLength++] = hex[0];
                        returnMe[outputLength++] = hex[1];
                        returnMe[outputLength++] = hex[2];
                        returnMe[outputLength++] = hex[3];
                    }
                    else
                    {
                        returnMe[outputLength++] = '\\';
                        returnMe[outputLength++] = 'x';
                        string hex = ((int)currentCharacter).ToString("x", CultureInfo.InvariantCulture).PadLeft(2, '0');
                        returnMe[outputLength++] = hex[0];
                        returnMe[outputLength++] = hex[1];
                    }
                }
                else
                {
                    // character does not need encoding
                    returnMe[outputLength++] = input[i];
                }
            }

            // Last step is to end the encoding with an apostrophe if flag is true.
            if (emitQuotes)
            {
                returnMe[outputLength++] = '\'';
            }

            return new string(returnMe, 0, outputLength);
        }

        /// <summary>
        /// Encodes input strings for use in Visual Basic Script.
        /// </summary>
        /// <param name="input">String to be encoded.</param>
        /// <returns>
        /// Encoded string for use in Visual Basic Script.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are 
        /// encoded using &#38;chrw(DECIMAL) notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>"alert"&#38;chrw(40)&#38;chrw(39)&#38;"XSS Attack"&#38;chrw(33)&#38;chrw(39)&#38;chrw(41)&#38;chrw(59)</description></item>
        /// <item><term>user@contoso.com</term><description>"user"&#38;chrw(64)&#38;"contoso.com"</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>"Anti-Cross Site Scripting Library"</description></item>
        /// </list></remarks>
        public static string VisualBasicScriptEncode(string input)
        {
            // Input validation: empty or null string condition
            if (string.IsNullOrEmpty(input))
            {
                return VbScriptEmptyString;
            }

            // Use a new char array.
            int outputLength = 0;
            int inputLength = input.Length;
            char[] encodedInput = new char[inputLength * 12]; // worst case length scenario

            // flag to surround double quotes around safe characters
            bool isInQuotes = false;

            for (int i = 0; i < inputLength; i++)
            {
                int currentCharacterAsInteger = input[i];
                char currentCharacter = input[i];
                if (safeListCodes[currentCharacterAsInteger] != null)
                {
                    // character needs to be encoded

                    // surround in quotes
                    if (isInQuotes)
                    {
                        // get out of quotes
                        encodedInput[outputLength++] = '"';
                        isInQuotes = false;
                    }

                    // adding "encoded" characters
                    string temp = "&chrw(" + ((uint)currentCharacter) + ")";
                    foreach (char ch in temp)
                    {
                        encodedInput[outputLength++] = ch;
                    }
                }
                else
                {
                    // character does not need encoding
                    // surround in quotes
                    if (!isInQuotes)
                    {
                        // add quotes to start
                        encodedInput[outputLength++] = '&';
                        encodedInput[outputLength++] = '"';
                        isInQuotes = true;
                    }

                    encodedInput[outputLength++] = input[i];
                }
            }

            // if we're inside of quotes, close them
            if (isInQuotes)
            {
                encodedInput[outputLength++] = '"';
            }

            // finally strip extraneous "&" from beginning of the string, if necessary and RETURN
            if (encodedInput.Length > 0 && encodedInput[0] == '&')
            {
                return new string(encodedInput, 1, outputLength - 1);
            }

            return new string(encodedInput, 0, outputLength);
        }

        /// <summary>
        /// Initializes the safe list.
        /// </summary>
        /// <returns>A two dimensional character array containing characters and their encoded values.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is necessary complexity.")]
        private static char[][] InitializeSafeList()
        {
            char[][] allCharacters = new char[65536][];
            for (int i = 0; i < allCharacters.Length; i++)
            {
                if (
                    (i >= 97 && i <= 122) ||        // a-z
                    (i >= 65 && i <= 90) ||         // A-Z
                    (i >= 48 && i <= 57) ||         // 0-9
                    i == 32 ||                      // space
                    i == 46 ||                      // .
                    i == 44 ||                      // ,
                    i == 45 ||                      // -
                    i == 95 ||                      // _
                    (i >= 256 && i <= 591) ||       // Latin,Extended-A,Latin Extended-B        
                    (i >= 880 && i <= 2047) ||      // Greek and Coptic,Cyrillic,Cyrillic Supplement,Armenian,Hebrew,Arabic,Syriac,Arabic,Supplement,Thaana,NKo
                    (i >= 2304 && i <= 6319) ||     // Devanagari,Bengali,Gurmukhi,Gujarati,Oriya,Tamil,Telugu,Kannada,Malayalam,Sinhala,Thai,Lao,Tibetan,Myanmar,eorgian,Hangul Jamo,Ethiopic,Ethiopic Supplement,Cherokee,Unified Canadian Aboriginal Syllabics,Ogham,Runic,Tagalog,Hanunoo,Buhid,Tagbanwa,Khmer,Mongolian   
                    (i >= 6400 && i <= 6687) ||     // Limbu, Tai Le, New Tai Lue, Khmer, Symbols, Buginese
                    (i >= 6912 && i <= 7039) ||     // Balinese         
                    (i >= 7680 && i <= 8191) ||     // Latin Extended Additional, Greek Extended        
                    (i >= 11264 && i <= 11743) ||   // Glagolitic, Latin Extended-C, Coptic, Georgian Supplement, Tifinagh, Ethiopic Extended    
                    (i >= 12352 && i <= 12591) ||   // Hiragana, Katakana, Bopomofo       
                    (i >= 12688 && i <= 12735) ||   // Kanbun, Bopomofo Extended        
                    (i >= 12784 && i <= 12799) ||   // Katakana, Phonetic Extensions         
                    (i >= 19968 && i <= 40899) ||   // Mixed japanese/chinese/korean
                    (i >= 40960 && i <= 42191) ||   // Yi Syllables, Yi Radicals        
                    (i >= 42784 && i <= 43055) ||   // Latin Extended-D, Syloti, Nagri        
                    (i >= 43072 && i <= 43135) ||   // Phags-pa         
                    (i >= 44032 && i <= 55215) /* Hangul Syllables */)
                {
                    allCharacters[i] = null;
                }
                else
                {
                    string integerStringValue = i.ToString(CultureInfo.InvariantCulture);
                    int integerStringLength = integerStringValue.Length;
                    char[] thisChar = new char[integerStringLength];
                    for (int j = 0; j < integerStringLength; j++)
                    {
                        thisChar[j] = integerStringValue[j];
                    }

                    allCharacters[i] = thisChar;
                }
            }

            return allCharacters;
        }
    }
}
