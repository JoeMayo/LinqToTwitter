using System;
using System.Linq;
using LinqToTwitter;

namespace Linq2TwitterDemos_WindowsStore.DataModel
{
    class UserStreamViewModel : StreamViewModel
    {
        public override async void OnStart(object obj)
        {
            int count = 0;

            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.User
                 select strm)
                .StartAsync(async strm =>
                {
                    Show(strm.Content);

                    if (count++ >= 5)
                        strm.CloseStream();
                });
        }
    }
}
