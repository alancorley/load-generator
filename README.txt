# HTTP Load Generator using ASP.NET Core 3.1

## Hello There! :wave: 
The purpose of this HTTP load generator console app util is to simulate http traffic against an API to help track down potential performance problems. 

Using modern C# and uses ASP.NET Core 3.1 async practices, the util will use parallel processing to hit a target RPS (requests per second). It continuously generate the requested load until the program has been shut down. It will handle/report on any errors from the server, and will generate a summary report including request/response metrics.

I've scoffolded this as a learning experiment and plan to build it out further as my free time permits. It is by no means meant to be a polished end result. 

In order to handle my requirements for being scalable and handle multiple concurrent requests, I implemented asynchronous loops to generate the api calls in parallel. You can set the number of concurrent threads and requests and see all kinds of statistics. While this test is limited to the number of cores on your development machine and not completely indicative of a live production environment, it's a decent simulation to replicate more realistic conditions and see how it scales under heavy load.

## Architecture
Although this is a console app, I did still use dependency injection. I am using HttpClientFactory to avoid having to manually manage pooling and lifetimes of each httpclient instance as well as helping to avoid socket exhaustion (using too many sockets too fast).

## Semaphore Object
I have implemented a Semaphore object using SemaphoreSlim for limiting the number of threads which can access a certain portion of the code as way to learn it and tinker around with Semaphore a little bit. This would be helpful to limit the amount of resouces as typically in production situations/environments we have finite resources. WaitAsync checks to see if there's any available slots for it to continue execution. 

## Architecture Considerations
- Ability to handle multiple concurrent requests
- Using consistent naming and URI conventions for ease of implementation and consumption
- Include useful status codes to give client feedback, whether it failed, passed or the request was wrong
- Implement Repository Pattern and interfaces to allow for consolidated query logic, easier unit testing, and ability to use a different datasource if ever needed
- Efficient use of server resources. To make this more scalable, I made these API calls asynchronous so that if ever hosted on an actual web server, it could handle multiple concurrent requests without issue. I am making sure that objects are removed from memory when they're completed, threads are destroyed, and other resources are managed as tightly as possible.

## Testing it out
It's simple, add your specific API details to the appsettigs.json file and Run the solution using Visual Studio. The console app will begin making http requests and when it's finished, it'll show a summary report.

## Future Wishlist
- Implement unit testing (started working on these but ended up putting a pin in it due to issues with DI)
- Show progress report as processing via callbacks
- Implement additional exception and error logging
- Add cancellation token handling
- Include additional options for specifying auth details like auth bearer token, etc.
- Include response time graph in summary results
- Fix unit tets and add addtional tests

Any feedback is greatly appreciated! If you have any suggestions to make it even better, let me know!

