using System.Diagnostics.CodeAnalysis;

namespace Letterbook.Core.Models;

/// <summary>
/// A Mention is when a Post is addressed to another individual Profile at any level of visibility.
/// </summary>
public class Mention : IEquatable<Mention>
{
	public PostId SourceId { get; set; }
	public ProfileId SubjectId { get; set; }
	public required Post Source { get; set; }
	public required Profile Subject { get; set; }
	public MentionVisibility Visibility { get; set; }

	private Mention()
	{
		Source = default!;
		Subject = default!;
	}

	[SetsRequiredMembers]
	public Mention(Post source, Profile subject, MentionVisibility visibility)
	{
		SourceId = source.Id;
		SubjectId = subject.Id;
		Source = source;
		Subject = subject;
		Visibility = visibility;
	}

	public bool Equals(Mention? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Subject.Equals(other.Subject);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((Mention)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Source, Subject);
	}

	public static bool operator ==(Mention? left, Mention? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(Mention? left, Mention? right)
	{
		return !Equals(left, right);
	}
}