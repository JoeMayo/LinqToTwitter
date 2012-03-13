using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

using LinqToTwitter.Common;

namespace LinqToTwitter
{
    public class Help
    {
        public static Help Create(XElement help)
        {
            return new Help
            {
                OK = 
                    help.Name == "ok" && !string.IsNullOrEmpty(help.Value) ? 
                        bool.Parse(help.Value) : 
                        false,
                Configuration = help.Name == "configuration" ?
                    new Configuration
                    {
                        ShortUrlLength = help.GetInt("short_url_length"),
                        ShortUrlLengthHttps = help.GetInt("short_url_length_https"),
                        NonUserNamePaths =
                            (from path in help.Element("non_username_paths").Elements("non_username_path")
                             select path.Value)
                            .ToList(),
                        PhotoSizeLimit = help.GetInt("photo_size_limit"),
                        MaxMediaPerUpload = help.GetInt("max_media_per_upload"),
                        CharactersReservedPerMedia = help.GetInt("characters_reserved_per_media"),
                        PhotoSizes =
                            (from photo in help.Element("photo_sizes").Elements()
                             select new PhotoSize
                             {
                                 Type = photo.Name.ToString(),
                                 Width = photo.GetInt("w"),
                                 Height = photo.GetInt("h"),
                                 Resize = photo.GetString("resize")
                             })
                            .ToList()
                    } :
                    null,
                Languages = help.Name == "languages" ?
                    (from lang in help.Elements("language")
                     select new Language
                     {
                         Name = lang.GetString("name"),
                         Code = lang.GetString("code"),
                         Status = lang.GetString("status")
                     })
                    .ToList() :
                    null
            };
        }

        /// <summary>
        /// Help Type (Test, Configuration, or Languages)
        /// </summary>
        public HelpType Type { get; set; }

        /// <summary>
        /// Will be true if help Test succeeds
        /// </summary>
        public bool OK { get; set; }

        /// <summary>
        /// Populated for Help Configuration query
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// List of languages, codes, and statuses
        /// </summary>
        public List<Language> Languages { get; set; }
    }
}
