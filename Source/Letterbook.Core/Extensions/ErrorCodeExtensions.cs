using Letterbook.Core.Exceptions;

namespace Letterbook.Core.Extensions;

public static class ErrorCodeExtensions
{
	public static ErrorCodes With(this ErrorCodes self, int flag)
	{
		return (ErrorCodes)((int)self | flag);
	}

	public static ErrorCodes With(this ErrorCodes self, ErrorCodes flag) => self.With((int)flag);

	public static bool Flagged(this ErrorCodes self, int flag)
	{
		return ((int)self & flag) == flag;
	}

	public static bool Flagged(this ErrorCodes self, ErrorCodes flag) => self.Flagged((int)flag);
}