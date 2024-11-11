namespace Letterbook.Docs.Files;

public class ProjectFilesException : Exception
{
	public ProjectFilesException()
	{
	}

	public ProjectFilesException(string? message) : base(message)
	{
	}

	public ProjectFilesException(string? message, Exception? innerException) : base(message, innerException)
	{
	}
}