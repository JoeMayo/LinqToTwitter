namespace TwitterTest;
 
interface
 
uses
  System.Linq,
  LinqToTwitter;

type
  TwitterTestClass = class
  public
    class method Main; 
  end;

implementation

class method TwitterTestClass.Main;
begin

  var TwitterCtx : TwitterContext := new TwitterContext;

  var Tweets :=
    from tweet in TwitterCtx.Status
    where tweet.Type = StatusType.Public
    select tweet;

  for tweet : Status in Tweets do begin
     System.Console.WriteLine(
        "User Name: {0}, Tweet: {1}",
            Tweet.User.Name,
            Tweet.Text);
  end;

end;
 
end.
