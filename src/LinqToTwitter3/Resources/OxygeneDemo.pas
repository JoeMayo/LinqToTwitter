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
//  var 
//	Auth : ApplicationOnlyAuthorizer;
//	Credentials : InMemoryCredentials;
//	TwitterCtx : TwitterContext;
begin
 
//  Credentials := new InMemoryCredentials;
//  Credentials.ConsumerKey := "";
//  Credentials.ConsumerSecret := "";
  
//  Auth := new ApplicationOnlyAuthorizer;
//  Auth.Credentials := Credentials;
  
//  Auth.Authorize(); 

//  TwitterCtx := new TwitterContext(Auth);
 
	var numbers: sequence of Integer := [1,2,3,4,5,6,7,8,9]; 
 
    var myquery := from c in numbers where (c >= 4) and (c <= 8) select c;
//  var SearchResponse :=
//    from srch in TwitterCtx.Search
//    where ((srch.Type == SearchType.Search) and
//	      (srch.Query == 'LINQ to Twitter'))
//    select srch;

//  System.Console.WriteLine(
//    "\nQuery: {0}\n", SearchResponse.SearchMetaData.Query);
	
//  for tweet : Status in SearchResponse.Statuses do begin
//     System.Console.WriteLine(
//        "User Name: {0}, tweet: {1}",
//            tweet.User.Name,
//            tweet.Text);
//  end;

end;
 
end.
