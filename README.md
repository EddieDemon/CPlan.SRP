Connection Planet - SRP Implementation.
=======================================

I know that this isn't completely functioning. So, please, help me out whenever you find issue.

Thank you in advance.

## Story behind
I was wondering how to secure my web-calls from my desktop applications to my website applications. To do this, I wanted to get it working with SRP6a. However, for some reason I seem not to be able to get the to work with Resource#3, the WSE implementation.

## Project setup
The project has been divided by three sub-projects.
- The client library
- The server library (Also referred to as the host.)
- A test console application.

The server library contains functions that are not needed by the client library. However, most of the used functions are used on both libraries, that's why the server library has a reference to the client library.

## Resources
- http://tools.ietf.org/html/rfc5054
- http://tools.ietf.org/html/rfc2945
- http://channel9.msdn.com/forums/sandbox/secure-remote-password-srp-in-wse