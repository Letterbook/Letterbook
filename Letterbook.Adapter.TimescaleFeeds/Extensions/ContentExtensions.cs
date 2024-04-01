using System.Text;
using Letterbook.Core.Models;

namespace Letterbook.Adapter.TimescaleFeeds.Extensions;

public static class ContentExtensions
{
	public static void AppendEntryRow(this StringBuilder sb, int row)
	{
		const int count = 9;
		var start = count * row;
		sb.Append('(');
		sb.AppendJoin(',', Enumerable.Range(start, count).Select(i => $"{{{i}}}"));
		sb.Append(')');
	}

	public static IEnumerable<object?> ToEntryValues(this IContentRef subject, Audience audience, Profile? boostedBy)
	{
		return new List<object?>
		{
			DateTime.UtcNow, // Time
            subject.Type,  // Type
            subject.FediId.ToString(),  // EntityId
            audience.FediId.ToString(),  // AudienceKey
            null,  // AudienceName
            subject.Creators.Select(c => c.FediId.ToString()).ToArray(),  // CreatedBy
            subject.Authority,  // Authority
            boostedBy?.FediId.ToString(),  // BoostedBy
            subject.CreatedDate,  // CreatedDate
        };
	}
}