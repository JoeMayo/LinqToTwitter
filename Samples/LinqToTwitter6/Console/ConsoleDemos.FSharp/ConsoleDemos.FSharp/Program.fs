open System
open LinqToTwitter
open System.Linq
open LinqToTwitter.OAuth

[<EntryPoint>]
let main argv =

    let credentialStore = {
        new SingleUserInMemoryCredentialStore() with
            member this.ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey)
            member this.ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
            member this.AccessToken = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessToken)
            member this.AccessTokenSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessTokenSecret)
    }

    let mutable auth = new SingleUserAuthorizer()
    auth.CredentialStore <- credentialStore
    let twitterCtx = new TwitterContext(auth)

    let recentSearchQuery = query {
        for search in twitterCtx.TwitterSearch do
        where (search.Type = SearchType.RecentSearch)
        where (search.Query = "#fsharp")
        select search
    }

    let response = recentSearchQuery.SingleOrDefault()

    for tweet in response.Tweets do
        printfn "%s - %s" tweet.ID tweet.Text

    0