This section explains what Twitter is, how the Twitter API works, and projects the idea of Twitter as a platform.

### What is Twitter?
Twitter is a micro-blogging service that allows people to post messages. The maximum length of each message is 140 characters. Twitter enables social networking by allowing people to follow and communicate with each other. Over time, this has proven to be a powerful communications medium because of its real-time nature. Whenever news breaks around the world, people are tweeting that news as it happens. For more information about Twitter, visit [Twitter.com](http://twitter.com/).

### How Does the Twitter API Work?
The Twitter API is built using [Representational state transfer](http://en.wikipedia.org/wiki/REST) (REST). Wikipedia defines REST as "a style of software architecture for distributed hypermedia systems", but to simplify it, REST is a Web service protocol built upon Hypertext Transfer Protocol (HTTP). You use the REST Web service by making an HTTP call with a URL and getting text back in JSON format.

This process used to be as simple as typing a URL into a browser, but that has changed. You must now use the OAuth protocol to let the user authorize your application to act on their behalf. In [[Securing Your Applications]], you can read an overview of how OAuth works with Twitter.

### What Does the Twitter API Do for Me?

Because of all the statuses being entered 24x7 all year long throughout the world, Twitter is an incredible source of information. There are many useful applications you can write to keep people informed about your industry and to see what people are saying. There are literally thousands of applications built upon the Twitter API today. If it's any indication, as of this writing, LINQ to Twitter has over 250,000 downloads and you can imagine that even a small fraction of that is a lot of apps. The API includes dozens of URLs with many options. This API is continuously growing, as is the 3rd party ecosystem. Twitter is more than an API that returns the last 20 tweets - it is a platform to create useful applications or integrate functionality with existing applications. I invite you to inspect the [Twitter API](http://dev.twitter.com/) and peruse the changelog to get a feel for its continuous growth and improvement.
