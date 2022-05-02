# CoreDiffApi
Provide two http endpoints that accept JSON base64 encoded binary data on both endpoints and a third endpoint to return the diffs between them.

### How it works:
Provide two http endpoints that receive JSON base64 encoded binary data:
```
[POST] http://localhost:44349/v1/diff/1/left
[Payload]
{
  "data": "Q29kaW5nIGlzIGZ1bi4="
}
[Result] 201
{
  "message":"ok"
}
[POST] http://localhost:44349/v1/diff/1/right
[Payload]
{
  "data": "Q29kaW5nIGlzIGZ1bi4="
}
[Result] 201
{
  "message":"ok"
}
```
Provide a third endpoint that shows diffs result in JSON format:
- Get: {host}/v1/diff/{id}
- The result can be:
- Equals
- ContentDoNotMatch (show difference(s))
- SizeDoNotMatch
```
[GET] http://localhost:44349/v1/diff/1
[Result] 200
{
  "DiffResultType" : "Equals",
}

```
### Assumptions
- Data persistence is not required
- Detail error message is not required
- Logging is not required

### Techonologies/Tools
- Aspnet Core 6.0
- Web Api
- EntityFramework Core InMemory
- xUnit
- Visual Studio 2022

### Possible Improvements
- Change In-Memory database to a NoSQL or a relational database
- Provide descriptive error message
- Increase code coverage
- Improve errors/exceptions handling
- Use Redis to cache results
- Distribute the application in Dock containers
- Implement load tests
- Add logs
