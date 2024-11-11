# Letterbook.Generators

Letterbook uses source code generators to facilitate using explicitly typed IDs. The basic idea is to wrap the actual
primitive identifier in a record with a more meaningful type. This gives us better compile time type safety,
particularly in situations where we're handling different kinds of domain models, but which have the same underlying
primary or secondary ID type. For instance, Posts and Threads, or Posts and Profiles. All of those are identifiable as a
UUIDv7, or as a Uri. That makes it easy to mix up values and create confusing bugs.

Now the compiler protects us in many of those scenarios.

## Source Code Generation

For clarity and performance, we prefer to use `record struct` as the wrapper type for our `TypedId` fields. Structs
can't inherit from a base type, which makes it harder to share logic between them. That leads to lots of boilerplate
that is highly redundant. This boilerplate is necessary to permit these types to serialize properly in various contexts.
So, we use source code generators to remove the boilerplate. We need this in three general contexts:

1. ASP.Net model binding. The `TypedId` structs are annotated directly with a converter attribute to support this.
2. Json serialization. The serializer for each `TypedId` is annotated with an attribute, and that attribute is used to locate and construct JsonConverter objects to add to the serializer context.
3. Entity Framework. The type converter for each `TypedId` is annotated with an attribute. That attribute is used to locate the type converter class and register it with EFCore during startup configuration. 

To create a `TypedId` type, you must define a partial record struct which implements `ITypedId<T>` where T is the type
of the underlying identifier. The interface is pretty minimal, but it does require some implemented methods. The
implementation is provided by generated code. So, usually all you need to do to define a new model and corresponding
`TypedId` is this:

```csharp
public partial record struct SomeModelId(Uuid7 id) : ITypedId<Uuid7> { }

public class SomeModel
{
    public SomeModelId id {get; set;}
}
```

Or similar, for other underlying types.

## Testing Source Generators

There are a few unit tests for the source generators. The generated code is not particularly complex, so we don't
currently bother with high fidelity tests of the generated code itself. Instead, we just test that the generator
produces the expected source files.

## Generated Output

At the moment, we do not emit generated source files back to the solution. They're generated on the fly when building
`Letterbook.Core`, and included in the build output directory. This hides all the boilerplate during normal development.
It does make them somewhat harder to review, however. If that becomes a problem, we can change it.
