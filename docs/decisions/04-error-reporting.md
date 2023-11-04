# Subject

Patterns and guidance to build consistent Exceptions with useful reporting and debugging information

## Status

- [ ] Decided
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

# Decision

In general, Letterbook has a set of application-defined Exceptions that all inherit from `Letterbook.Core.CoreException`. In addition, these exceptions should use a builder pattern to facilitate collecting extra information and including it in a standard way on our Exceptions.

## Impact

* Exceptions should include a message. This is just universally good practice.
* Exceptions should set the `Source` field, with information about the origin point of the exception. This can be used in logs and error messages, in the cases we don't share the whole stack trace. See below about `[Caller*]` attributes.
* Exceptions should set the `HResult` with the applicable error code flags.
* Exceptions should include other relevant data in the `Data` dictionary. The expectation is that the data dictionary could be used by telemetry, and for debugging, but shouldn't be exposed to users via error messages.
* To keep all of this consistent, custom exceptions should usually define static builder methods and other code should use those rather than constructing their own Exception objects.

## Context

We've started doing this a little bit already, but it's valuable to write down the decision. For example, `Letterbook.Core.CoreException` implements the builder pattern, as well as sets up some error flags.

The builders all make use of `[Caller*]` attributes on the builder method arguments. These attributes cause the compiler to replace default values with a computed compile time constant. This allows us to collect information about the calling site of these methods statically, and with no performance cost. That way we can record the method and location where these builders are called from. That's a lot easier than trying to intercept and examine a stack trace, and faster than using reflection to get the information.

This guidance is meant to be in keeping with Microsoft's recommended best practices. See the docs here https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions#in-custom-exceptions-provide-additional-properties-as-needed

## Discussion

This ADR is as much to collect discussions about help ourselves in the future with good error reporting as it is to codify this specific practice. Any suggestion or feedback is welcome.