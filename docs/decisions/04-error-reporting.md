# Subject

Patterns and guidance to build consistent Exceptions with useful reporting and debugging information

## Status

- [ ] Decided
- [ ] Decided Against
- [ ] Deferred
- [ ] Superseded

# Decision

In general, Letterbook has a set of application-defined Exceptions that all inherit from `Letterbook.Core.CoreException`. In addition, these exceptions should use a factory pattern to facilitate collecting extra information and including it in a standard way on our Exceptions.

## Impact

* Exceptions should include a message. This is just universally good practice.
* Exceptions should set the `Source` field, with information about the origin point of the exception. See below about `[Caller*]` attributes.
* Exceptions should set the `HResult` with the applicable error code flags.
* Exceptions should include other relevant data in the `Data` dictionary. The expectation is that the data dictionary could be used by telemetry, and for debugging, but shouldn't be exposed to users via error messages.
* To keep all of this consistent, custom exceptions should usually define static builder methods and other code should use those rather than constructing their own Exception objects.

## Context

This guidance is meant to be in keeping with Microsoft's recommended best practices. See the docs here https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions#in-custom-exceptions-provide-additional-properties-as-needed

## Discussion

A summary of the comments/issues/concerns that led up to the decision