using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace AccountActivityDemo.Controllers
{
    public class AccountActivityController : ApiController
    {
        // GET api/values/5
        public object Get(string crc_token)
        {
            string key = ""; // Your Twitter Consumer Secret
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] crcBytes = Encoding.UTF8.GetBytes(crc_token);

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(crcBytes);
            var base64Hmac = Convert.ToBase64String(hash);

            //string response = $@"{{ ""response_token"": ""{base64Hmac}"" }}";
            var response = new
            {
                response_token = "sha256=" + base64Hmac
            };

            return response;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }
    }
}
