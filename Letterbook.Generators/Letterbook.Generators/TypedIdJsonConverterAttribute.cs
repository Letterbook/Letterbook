using System;

namespace Letterbook.Generators;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class TypedIdJsonConverterAttribute : Attribute { }