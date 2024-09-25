using AutoMapper;
using Letterbook.Core;
using Letterbook.Core.Models;
using Letterbook.Core.Models.Mappers;
using Letterbook.Workers.Contracts;
using Letterbook.Workers.Publishers;
using MassTransit;
using Claim = System.Security.Claims.Claim;

namespace Letterbook.Workers.Consumers
{
	public class TimelineConsumer : IConsumer<PostEvent>
	{
		private readonly ILogger<TimelineConsumer> _logger;
		private readonly ITimelineService _timelines;
		private readonly IProfileService _profiles;
		private readonly Mapper _mapper;

		public TimelineConsumer(ILogger<TimelineConsumer> logger, ITimelineService timelines, IProfileService profiles,
			MappingConfigProvider maps)
		{
			_logger = logger;
			_timelines = timelines;
			_profiles = profiles;
			_mapper = new Mapper(maps.Posts);
		}

		public async Task Consume(ConsumeContext<PostEvent> context)
		{
			_logger.LogInformation("Handling PostEvent {EventType} for {PostId}", context.Message.Type, context.Message.Subject);
			var post = _mapper.Map<Post>(context.Message.NextData);
			switch (context.Message.Type)
			{
				case nameof(PostEventPublisher.Created):
					return;
				case nameof(PostEventPublisher.Deleted):
					await _timelines.As(context.Message.Claims.Select(c => (Claim)c)).HandleDelete(post);
					return;
				case nameof(PostEventPublisher.Updated):
					if (post.PublishedDate is null) return;
					if (context.Message.PrevData is not null)
					{
						var prev = _mapper.Map<Post>(context.Message.NextData);
						await _timelines.As(context.Message.Claims.Select(c => (Claim)c)).HandleUpdate(post, prev);
					}
					else
						_logger.LogError("Couldn't update Post {PostId} in timeline due to unknown previous state", post.GetId25());

					return;
				case nameof(PostEventPublisher.Published):
					await _timelines.As(context.Message.Claims.Select(c => (Claim)c)).HandlePublish(post);
					return;
				case nameof(PostEventPublisher.Liked):
					return;
				case nameof(PostEventPublisher.Shared):
					_logger.LogDebug("Sender: {Id}", context.Message.Sender);
					if (context.Message.Sender is { } senderId
					    && await _profiles.As(context.Message.Claims.Select(c => (Claim)c)).LookupProfile(senderId) is { } sender)
					{
						await _timelines.As(context.Message.Claims.Select(c => (Claim)c)).HandleShare(post, sender);
					}
					else
						_logger.LogError("Couldn't add shared Post {PostId} to timeline due to missing sender info for {ProfileId}",
							post.GetId25(), context.Message.Sender);

					return;
				default:
					_logger.LogError("Couldn't handle unknown PostEvent {EventType}", context.Message.Type);
					return;
			}
		}
	}
}