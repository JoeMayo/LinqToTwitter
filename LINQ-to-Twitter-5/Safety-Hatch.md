### Safety Hatch

If something isn't quite right or available, you still have a work around. For example, what if the Twitter API adds a new parameter to an endpoint; an entirely new endpoint; or a new set of beta endpoints that aren't available in the latest version of LINQ to Twitter? What if LINQ to Twitter has a bug that isn't fixed in the latest release? You aren't stuck.

The safety hatch is Raw queries and commands. While a bit more work, this approach is flexible and still helps you avoid much low-level plumbing code, such as HTTP communication and OAuth.

#### [Raw Queries](Safety-Hatch/Raw-Queries.md)

Perform a query on any HTTP GET endpoint in the Twitter API.

#### [Raw Commands](Safety-Hatch/Raw-Commands.md)

Communicate with any HTTP POST endpoint in the Twitter API.