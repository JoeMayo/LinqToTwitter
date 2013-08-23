using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTwitter.Security
{
    public abstract class AuthorizerBase
    {
        public async Task<string> GetRequestToken()
        {
            await Task.Delay(1);
            return "";
        }

        public async Task Authorize()
        {
            await Task.Delay(1);
        }

        public async Task<string> GetAccessToken()
        {
            await Task.Delay(1);
            return "";
        }
    }
}
