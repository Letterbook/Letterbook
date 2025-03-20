using ActivityPub.Types.AS;
using ActivityPub.Types.Util;

namespace Letterbook.Adapter.ActivityPub.Test;

public class ExtensionTests
{
	[Fact(DisplayName = "Should get the linkable object ID")]
	public void ShouldGetId()
	{
		var expected = "https://example.com";
		var ext = new Linkable<ASObject>(new ASObject() { Id = expected });

		Assert.True(ext.TryGetId(out var id));
		Assert.Equal(expected, id.OriginalString);
	}

	[Fact(DisplayName = "Should return false when no id")]
	public void ShouldNotGetId()
	{
		var ext = new Linkable<ASObject>(new ASObject());

		Assert.False(ext.TryGetId(out _));
	}

	[Fact(DisplayName = "Should get the object ID")]
	public void ShouldGetObjectId()
	{
		var expected = "https://example.com";
		var ext = new ASObject() { Id = expected };

		Assert.True(ext.TryGetId(out var id));
		Assert.Equal(expected, id.OriginalString);
	}

	[Fact(DisplayName = "Should get the href")]
	public void ShouldGetLink()
	{
		var expected = "https://example.com";
		var ext = new Linkable<ASObject>(new ASLink() { HRef = expected });

		Assert.True(ext.TryGetId(out var id));
		Assert.Equal(expected, id.OriginalString);
	}

	[Fact(DisplayName = "Should throw ArgumentOutOfRange when there's no non-null")]
	public void ShouldThrowOutOfRange()
	{
		string? s = null;
		Assert.Throws<ArgumentOutOfRangeException>(() => Extensions.NotNull(s));
		Assert.Throws<ArgumentOutOfRangeException>(() => Extensions.NotNull());
	}

	[Fact(DisplayName = "Should return the first non-null value")]
	public void ShouldGetNonNull()
	{
		var s = "right";
		Assert.Equal(s, Extensions.NotNull(s));
		Assert.Equal(s, Extensions.NotNull(null, s));
		Assert.Equal(s, Extensions.NotNull(null, s, null));
		Assert.Equal(s, Extensions.NotNull(null, s, null, "other"));
		Assert.Equal(s, Extensions.NotNull(s, "wrong", null, "other"));
	}

	[Fact(DisplayName = "Should get IDs from a list of ASObject")]
	public void ShouldGetObjectListIds()
	{
		var list = new[] { new ASObject() { Id = "https://test.example" }, new ASObject() { Id = "https://test.example/2" } };
		var actual = list.SelectIds();

		Assert.Equal(2, actual.Count());
	}

	[Fact(DisplayName = "Should get IDs from a list of ASLink")]
	public void ShouldGetLinkListIds()
	{
		var list = new[] { new ASLink() { HRef = "https://test.example" }, new ASLink() { HRef = "https://test.example/2" } };
		var actual = list.SelectIds();

		Assert.Equal(2, actual.Count());
	}
}