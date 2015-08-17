using System.Linq;
using LinqToTwitter;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace UwpSamples
{
    class SampleStreamViewModel : StreamViewModel
    {
        public override async void OnStart(object obj)
        {
            int count = 0;

            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            try
            {
                await
                    (from strm in twitterCtx.Streaming
                     where strm.Type == StreamingType.Sample
                     select strm)
                    .StartAsync(async strm =>
                    {
                            object tweetJson = JsonConvert.DeserializeObject(strm.Content);
                            await ShowAsync(tweetJson.ToString());

                        if (count++ >= 50)
                            strm.CloseStream();
                    });
            }
            catch (IOException ex)
            {
                // Twitter might have closed the stream,
                // which they do sometimes. You should
                // restart the stream, but be sure to
                // read Twitter documentation on stream
                // back-off strategies to prevent your
                // app from being blocked.
                await ShowAsync(ex.ToString());
            }
        }
    }
}
