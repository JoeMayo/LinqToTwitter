using System.Linq;
using LinqToTwitter;

namespace UwpSamples
{
    class SampleStreamViewModel : StreamViewModel
    {
        public override async void OnStart(object obj)
        {
            int count = 0;

            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Sample
                 select strm)
                .StartAsync(async strm =>
                {
                    await ShowAsync(strm.Content);

                    if (count++ >= 50)
                        strm.CloseStream();
                });
        }
    }
}
