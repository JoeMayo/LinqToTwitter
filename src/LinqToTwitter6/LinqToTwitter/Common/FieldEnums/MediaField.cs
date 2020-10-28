namespace LinqToTwitter.Common
{
    /// <summary>
    /// Fields that can be expanded on <see cref="Media"/>
    /// </summary>
    public class MediaField
    {
        /// <summary>
        /// All expandable fields
        /// </summary>
        public const string AllFields = 
            "duration_ms,height,media_key,preview_image_url," +
            "type,url,width,public_metrics,non_public_metrics," +
            "organic_metrics,promoted_metrics";

        /// <summary>
        /// All expandable fields, except those requiring permissions
        /// </summary>
        public const string AllFieldsExceptPermissioned =
            "duration_ms,height,media_key,preview_image_url," +
            "type,url,width,public_metrics";

        /// <summary>
        /// duration_ms
        /// </summary>
        public const string Duration = "duration_ms";


        /// <summary>
        /// height
        /// </summary>
        public const string Height = "height";


        /// <summary>
        /// media_key
        /// </summary>
        public const string MediaKey = "media_key";


        /// <summary>
        /// preview_image_url
        /// </summary>
        public const string PreviewImageUrl = "preview_image_url";


        /// <summary>
        /// type
        /// </summary>
        public const string Type = "type";


        /// <summary>
        /// url
        /// </summary>
        public const string Url = "url";


        /// <summary>
        /// width
        /// </summary>
        public const string Width = "width";


        /// <summary>
        /// public_metrics
        /// </summary>
        public const string PublicMetrics = "public_metrics";


        /// <summary>
        /// non_public_metrics
        /// </summary>
        public const string NonPublicMetrics = "non_public_metrics";


        /// <summary>
        /// organic_metrics
        /// </summary>
        public const string OrganicMetrics = "organic_metrics";


        /// <summary>
        /// promoted_metrics
        /// </summary>
        public const string PromotedMetrics = "promoted_metrics";
    }
}
