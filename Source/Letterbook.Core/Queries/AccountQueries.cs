using Microsoft.EntityFrameworkCore;

namespace Letterbook.Core.Queries;

public static class AccountQueries
{
	public static IQueryable<Models.Account> WithProfiles(this IQueryable<Models.Account> query)
	{
		return query
			.Include(account => account.LinkedProfiles)
			.ThenInclude(l => l.Profile)
			.AsSplitQuery();
	}
}