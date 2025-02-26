using AutoMapper;
using Letterbook.Core.Adapters;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Dto;
using Letterbook.Core.Models.Mappers;
using Letterbook.Workers.Contracts;
using MassTransit;
using Claim = System.Security.Claims.Claim;

namespace Letterbook.Workers.Publishers;

public class ModerationEventPublisher : IModerationEventPublisher
{
	private readonly IBus _bus;
	private readonly Mapper _mapper;

	public ModerationEventPublisher(IBus bus, MappingConfigProvider maps)
	{
		_bus = bus;
		_mapper = new Mapper(maps.ModerationReports);
	}

	/// <inheritdoc />
	public async Task Created(ModerationReport report, ProfileId author, IEnumerable<Claim> claims)
	{
		var message = Message(report, nameof(Created), claims, author);
		await _bus.Publish(message, c => c.SetCustomHeaders(nameof(Created)));
	}

	/// <inheritdoc />
	public async Task Assigned(ModerationReport report, Guid moderator, IEnumerable<Claim> claims)
	{
		var message = Message(report, nameof(Assigned), claims, moderator: moderator);
		await _bus.Publish(message, c => c.SetCustomHeaders(nameof(Assigned)));
	}

	/// <inheritdoc />
	public async Task Closed(ModerationReport report, Guid moderator, IEnumerable<Claim> claims)
	{
		var message = Message(report, nameof(Closed), claims, moderator: moderator);
		await _bus.Publish(message, c => c.SetCustomHeaders(nameof(Closed)));
	}

	/// <inheritdoc />
	public async Task Reopened(ModerationReport report, Guid moderator, IEnumerable<Claim> claims)
	{
		var message = Message(report, nameof(Reopened), claims, moderator: moderator);
		await _bus.Publish(message, c => c.SetCustomHeaders(nameof(Reopened)));
	}

	private ModerationReportEvent Message(ModerationReport report, string type, IEnumerable<Claim> claims, ProfileId author = default,
		Guid moderator = default)
	{
		return new ModerationReportEvent
		{
			Author = author,
			Moderator = moderator,
			NextData = _mapper.Map<FullModerationReportDto>(report),
			Claims = claims.MapDto(),
			Type = type
		};
	}
}