#nullable disable

using System.Collections.Generic;

namespace LinqToTwitter.Net
{
    // This is the shape of any potential error coming from the new Twitter API
    // TwitterErrorDetails
    //{
    //	"errors": [
    //		{
    //			"parameters": {
    //				"query": []
    //          },
    //			"message": "Request parameter `query` can not be empty"
    //		},
    //		{
    //			"parameters": {
    //				"q": [
    //					"LINQ%20to%20Twitter"
    //				]
    //			},
    //			"message": "[q] is not one of [query,start_time,end_time,since_id,until_id,max_results,next_token,expansions,tweet.fields,media.fields,poll.fields,place.fields,user.fields]"
    //		}
    //	],
    //	"title": "Invalid Request",
    //	"detail": "One or more parameters to your request was invalid.",
    //	"type": "https://api.twitter.com/labs/2/problems/invalid-request"
    //}

    public class TwitterErrorDetails
    {
        public Error[] Errors { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Type { get; set; }
    }


    public class Error
    {
        public Dictionary<string, string[]> Parameters { get; set; }
        public string Message { get; set; }
    }
}
