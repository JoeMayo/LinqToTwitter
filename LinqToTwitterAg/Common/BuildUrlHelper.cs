using System.Collections.Generic;
using System.IO;

namespace LinqToTwitter
{
    public class BuildUrlHelper
    {
        /// <summary>
        /// makes ID parameter part of the URL
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <param name="url">original url</param>
        /// <returns>transformed URL with ID</returns>
        public static string TransformIDUrl(Dictionary<string, string> parameters, string url)
        {
            return TransformParameterUrl(parameters, "ID", url);
        }

        /// <summary>
        /// makes a parameter part of the URL
        /// </summary>
        /// <param name="parameters">parameter list</param>
        /// <param name="url">original url</param>
        /// <returns>transformed URL with ID</returns>
        public static string TransformParameterUrl(Dictionary<string, string> parameters, string key, string url)
        {
            if (parameters.ContainsKey(key))
            {
                var fileExtension = Path.GetExtension(url);
                url = url.Replace(fileExtension, "/" + parameters[key] + fileExtension);
            }
            return url;
        }
    }
}
