/***********************************************************
 * Credits:
 * 
 * Eran Sandler -
 * OAuthBase Class
 * 
 * http://oauth.googlecode.com/svn/code/csharp/
 * 
 * Shannon Whitley -
 * Example of how to use modified version of
 * Eran Sandler's OAuthBase class in C#
 * 
 * http://www.voiceoftech.com/swhitley/?p=681
 * 
 * Joe Mayo -
 * 
 * Modified 5/3/09
 ***********************************************************/

namespace LinqToTwitter
{
    /// <summary>
    /// Provides a predefined set of algorithms that are supported officially by the protocol
    /// </summary>
    public enum OAuthSignatureTypes
    {
        HMACSHA1,
        PLAINTEXT,
        RSASHA1
    }
}
