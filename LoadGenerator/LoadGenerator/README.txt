DETAILS:
The purpose of this HTTP load generator console app util is to simulate http traffic against an API to help track down potential perf problems. 

Using modern C# async practices, the util will use parallel processing to hit a target RPS (requests per second). It continuously generate the requested load until the program
has been shut down. It will handle/report on any errors from the server, and will generate a summary report including request/response metrics.

INPUTS: hostname, path, and target requests per second