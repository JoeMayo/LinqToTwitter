using System;
using LinqToTwitter.Common;
using LitJson;

namespace LinqToTwitter
{
    public class StatusMetaData
    {
        public StatusMetaData() { }
        public StatusMetaData(JsonData mdJson)
        {
            ResultType = mdJson.GetValue<string>("result_type");
            IsoLanguageCode = mdJson.GetValue<string>("iso_language_code");
        }

        public string ResultType { get; set; }

        public string IsoLanguageCode { get; set; }
    }
}
