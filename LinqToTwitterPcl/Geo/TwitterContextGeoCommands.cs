using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        public async Task<Place> CreatePlaceAsync(string name, string containedWithin, string token, double latitude, double longitude, string attribute)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name is a required parameter.", "name");

            if (string.IsNullOrWhiteSpace(containedWithin))
                throw new ArgumentException("containedWithin is a required parameter.", "containedWithin");

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("token is a required parameter.", "token");

            if (latitude == 0)
                throw new ArgumentException("latitude is a required parameter.", "latitude");

            if (longitude == 0)
                throw new ArgumentException("longitude is a required parameter.", "longitude");

            string placeUrl = BaseUrl + "geo/place.json";

            var createParams = new Dictionary<string, string>
                {
                    { "name", name },
                    { "contained_within", containedWithin },
                    { "token", token },
                    { "lat", latitude.ToString(Culture.US) },
                    { "long", longitude.ToString(Culture.US) },
                    { "attribute", name }
                };

            var reqProc = new GeoRequestProcessor<Place>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<Place>(
                    placeUrl,
                    createParams);

            return reqProc.ProcessActionResult(resultsJson, GeoAction.CreatePlace);
        }

    }
}
