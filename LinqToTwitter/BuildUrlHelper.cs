using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            if (parameters.ContainsKey("ID"))
            {
                url = url.Replace(".xml", "/" + parameters["ID"] + ".xml");
            }
            return url;
        }
    }
}
