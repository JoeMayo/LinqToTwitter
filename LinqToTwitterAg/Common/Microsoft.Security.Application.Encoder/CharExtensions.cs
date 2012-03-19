namespace LinqToTwitter.Common
{
    public static class CharExtensions
    {
        internal const char HighSurrogateStart = '\ud800';
        internal const char HighSurrogateEnd = '\udbff';
        internal const char LowSurrogateStart = '\udc00';
        internal const char LowSurrogateEnd = '\udfff';

        public static bool IsHighSurrogate(this char c)
        {
            return ((c >= HighSurrogateStart) && (c <= HighSurrogateEnd));
        }

        public static bool IsLowSurrogate(this char c)
        {
            return ((c >= LowSurrogateStart) && (c <= LowSurrogateEnd));
        }
    }
}
