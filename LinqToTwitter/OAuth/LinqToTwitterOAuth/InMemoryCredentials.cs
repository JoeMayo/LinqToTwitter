using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public class InMemoryCredentials : IOAuthCredentials
    {
        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string AccessToken { get; set; }

        public void Load(string credentialString)
        {
            string[] credentials = credentialString.Split(',');

            ConsumerKey = credentials[0];
            ConsumerSecret = credentials[1];
            AccessToken = credentials[2];
        }

        public override string ToString()
        {
            return ConsumerKey + "," + ConsumerSecret + "," + AccessToken;
        }
    }
}
