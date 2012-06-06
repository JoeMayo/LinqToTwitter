using System;
using System.Collections.Generic;
using System.Globalization;

namespace LinqToTwitter
{
    public static class GeoExtensions
    {
        public static Place CreatePlace(this TwitterContext ctx, string name, string containedWithin, string token, double latitude, double longitude, string attribute)
        {
            return CreatePlace(ctx, name, containedWithin, token, latitude, longitude, attribute, null);
        }

        public static Place CreatePlace(this TwitterContext ctx, string name, string containedWithin, string token, double latitude, double longitude, string attribute, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name is a required parameter.", "name");
            }

            if (string.IsNullOrEmpty(containedWithin))
            {
                throw new ArgumentException("containedWithin is a required parameter.", "containedWithin");
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("token is a required parameter.", "token");
            }

            if (latitude == 0)
            {
                throw new ArgumentException("latitude is a required parameter.", "latitude");
            }

            if (longitude == 0)
            {
                throw new ArgumentException("longitude is a required parameter.", "longitude");
            }

            string placeUrl = ctx.BaseUrl + "geo/place.json";

            var createParams = new Dictionary<string, string>
                {
                    { "name", name },
                    { "contained_within", containedWithin },
                    { "token", token },
                    { "lat", latitude.ToString(CultureInfo.InvariantCulture) },
                    { "long", longitude.ToString(CultureInfo.InvariantCulture) },
                    { "attribute", name }
                };

            var reqProc = new GeoRequestProcessor<Place>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.ExecuteTwitter(
                    placeUrl,
                    createParams,
                    response => reqProc.ProcessActionResult(response, GeoAction.CreatePlace));

            Place results = reqProc.ProcessActionResult(resultsJson, GeoAction.CreatePlace);
            return results;
        }

    }
}
