// ReSharper disable BuiltInTypeReferenceStyle
namespace Letterbook.Core.Models;

[Flags]
public enum ProfilePermission : UInt64
{
	None = 0,
	// TODO: RBAC vs CBAC, and also define those access levels
	All = UInt64.MaxValue,
}