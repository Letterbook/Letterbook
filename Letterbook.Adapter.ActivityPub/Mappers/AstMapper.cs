using System.Security.Cryptography;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Collection;
using ActivityPub.Types.AS.Extended.Actor;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Util;
using AutoMapper;
using JetBrains.Annotations;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Exceptions;
using Letterbook.Core.Extensions;
using Letterbook.Core.Models;
using Medo;
using Org.BouncyCastle.Crypto.Parameters;
using PemReader = Org.BouncyCastle.OpenSsl.PemReader;

namespace Letterbook.Adapter.ActivityPub.Mappers;

/// <summary>
/// Map ActivityPubSharp objects to Model types
/// </summary>
public static class AstMapper
{
	/// <summary>
	/// Map from AS types to Model types
	/// </summary>
	public static MapperConfiguration Default = new(cfg =>
	{
		ConfigureBaseTypes(cfg);
		ConfigureKeyTypes(cfg);
		FromActor(cfg);
		FromNote(cfg);
		FromASType(cfg);
	});


	/// <summary>
	/// Map Posts and Content to AS types
	/// </summary>
	public static MapperConfiguration FromPost = new(cfg =>
	{
		ConfigureUriTypes(cfg);
		ConfigureDateTypes(cfg);
		ConfigureContentTypes(cfg);

		cfg.CreateMap<Content, NaturalLanguageString>(MemberList.None)
			.ForMember(d => d.DefaultValue, opt => opt.MapFrom(s => s.Html));

		cfg.CreateMap<string, NaturalLanguageString>(MemberList.None)
			.ConstructUsing(s => new NaturalLanguageString()
			{
				DefaultValue = s
			});

		cfg.CreateMap<Models.Post, NoteObject>(MemberList.Destination)
			.ForMember(d => d.Id, opt => opt.MapFrom(s => s.FediId))
			.ForMember(d => d.AttributedTo, opt => opt.MapFrom(s => s.Creators.Select(p => p.FediId)))
			.ForMember(d => d.Audience, opt => opt.MapFrom(s => s.Audience.Select(a => a.FediId)))
			.ForMember(d => d.Context, opt => opt.MapFrom(s => s.Thread.FediId))
			.ForMember(d => d.Published, opt => opt.MapFrom(s => s.PublishedDate))
			.ForMember(d => d.Updated, opt => opt.MapFrom(s => s.UpdatedDate))
			.ForMember(d => d.Content, opt => opt.MapFrom(s => s.Contents.Order().FirstOrDefault()))
			.ForMember(d => d.MediaType,
				opt => opt.MapFrom<MediaTypeResolver, Content?>(s => s.Contents.Order().FirstOrDefault()))
			.ForMember(d => d.Attachment,
				opt => opt.MapFrom<AttachedContentResolver, IEnumerable<Content>>(s => s.Contents.Order().Skip(1)))
			.ForMember(d => d.Likes, opt => opt.MapFrom(p => p.Likes))
			.ForMember(d => d.Replies, opt => opt.MapFrom<PostRepliesResolver, Uri?>(p => p.Replies))
			.ForMember(d => d.Preview, opt => opt.MapFrom(p => p.Preview))
			.ForMember(d => d.Shares, opt => opt.MapFrom(p => p.Shares))
			.ForMember(d => d.To,
				opt => opt.MapFrom(p => p.AddressedTo.Where(m => m.Visibility == MentionVisibility.To).Select(m => m.Subject.FediId)))
			.ForMember(d => d.CC,
				opt => opt.MapFrom(p => p.AddressedTo.Where(m => m.Visibility == MentionVisibility.Cc).Select(m => m.Subject.FediId)))
			.ForMember(d => d.InReplyTo, opt => opt.MapFrom<FediIdResolver, Post?>(s => s.InReplyTo))
			.ForMember(d => d.Id, opt => opt.MapFrom(s => s.FediId))
			.ForMember(d => d.Source, opt => opt.Ignore())
			.ForMember(d => d.BCC, opt => opt.Ignore())
			.ForMember(d => d.BTo, opt => opt.Ignore())
			.ForMember(d => d.Generator, opt => opt.Ignore())
			.ForMember(d => d.Icon, opt => opt.Ignore())
			.ForMember(d => d.Image, opt => opt.Ignore())
			.ForMember(d => d.Location, opt => opt.Ignore())
			.ForMember(d => d.Tag, opt => opt.Ignore())
			.ForMember(d => d.Url, opt => opt.Ignore())
			.ForMember(d => d.Duration, opt => opt.Ignore())
			.ForMember(d => d.StartTime, opt => opt.Ignore())
			.ForMember(d => d.EndTime, opt => opt.Ignore())
			.ForMember(d => d.Type, opt => opt.Ignore())
			.ForMember(d => d.JsonLDContext, opt => opt.Ignore())
			.ForMember(d => d.Name, opt => opt.Ignore());
	});

	/// <summary>
	/// Map Profiles to AS types
	/// </summary>
	public static MapperConfiguration Profile = new(cfg =>
	{
		ConfigureUriTypes(cfg);
		ConfigureDateTypes(cfg);
		ConfigureKeyTypes(cfg);
		ConfigureProfile(cfg);
	});

	private static void ConfigureProfile(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<Models.Profile, ProfileActor>(MemberList.None)
			.ConstructUsing((profile, _) => profile.Type switch
			{
				ActivityActorType.Application => new ProfileApplicationActor { Inbox = profile.Inbox, Outbox = profile.Outbox },
				ActivityActorType.Group => new ProfileGroupActor { Inbox = profile.Inbox, Outbox = profile.Outbox },
				ActivityActorType.Organization => new ProfileOrganizationActor { Inbox = profile.Inbox, Outbox = profile.Outbox },
				ActivityActorType.Person => new ProfilePersonActor { Inbox = profile.Inbox, Outbox = profile.Outbox },
				ActivityActorType.Service => new ProfileServiceActor { Inbox = profile.Inbox, Outbox = profile.Outbox },
				ActivityActorType.Unknown => new ProfilePersonActor { Inbox = profile.Inbox, Outbox = profile.Outbox },
				_ => new ProfilePersonActor { Inbox = profile.Inbox, Outbox = profile.Outbox },
			})
			.ForMember(d => d.Id, opt => opt.MapFrom(s => s.FediId))
			.ForMember(d => d.Updated, opt => opt.MapFrom(s => s.Updated))
			.ForMember(d => d.Published, opt => opt.MapFrom(s => s.Created))
			.ForMember(d => d.Summary, opt => opt.MapFrom(s => s.Description))
			.ForMember(d => d.PreferredUsername, opt => opt.MapFrom(s => s.Handle))
			.ForMember(d => d.Name, opt => opt.MapFrom(s => s.DisplayName))
			.ForMember(d => d.SharedInbox, opt => opt.MapFrom(s => s.SharedInbox))
			.ForMember(d => d.PublicKeys, opt => opt.MapFrom(s => s.Keys));
	}

	private static void ConfigureContentTypes(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<Content, ASObject>(MemberList.None)
			.ForMember(d => d.Id, opt => opt.MapFrom(s => s.FediId))
			.ForMember(d => d.AttributedTo, opt => opt.MapFrom<FediIdResolver, IEnumerable<Models.Profile>>(s => s.Post.Creators))
			.ForMember(d => d.Published, opt => opt.MapFrom(s => s.Post.PublishedDate))
			.ForMember(d => d.Updated, opt => opt.MapFrom(s => s.Post.UpdatedDate))
			.ForMember(d => d.MediaType, opt => opt.MapFrom(s => s.ContentType))
			.ForMember(d => d.Type, opt => opt.Ignore())
			.ForSourceMember(s => s.Id, opt => opt.DoNotValidate())
			.ForSourceMember(s => s.Post, opt => opt.DoNotValidate())
			.ForSourceMember(s => s.SortKey, opt => opt.DoNotValidate());

		cfg.CreateMap<Note, ASObject>(MemberList.Source)
			.ConstructUsing(_ => new NoteObject())
			.IncludeBase<Content, ASObject>()
			.ForMember(d => d.Content, opt => opt.MapFrom(s => s.Html))
			.ForSourceMember(s => s.SourceText, opt => opt.DoNotValidate())
			.ForSourceMember(s => s.SourceContentType, opt => opt.DoNotValidate())
			;
	}

	private static void FromActor(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<ProfilePersonActor, Models.Profile>()
			.IncludeBase<ProfileActor, Models.Profile>();
		cfg.CreateMap<ProfileApplicationActor, Models.Profile>()
			.IncludeBase<ProfileActor, Models.Profile>();
		cfg.CreateMap<ProfileServiceActor, Models.Profile>()
			.IncludeBase<ProfileActor, Models.Profile>();
		cfg.CreateMap<ProfileGroupActor, Models.Profile>()
			.IncludeBase<ProfileActor, Models.Profile>();
		cfg.CreateMap<ProfileOrganizationActor, Models.Profile>()
			.IncludeBase<ProfileActor, Models.Profile>();

		cfg.CreateMap<ProfileActor, Models.Profile>(MemberList.Destination)
			.ConstructUsing(_ => Models.Profile.CreateEmpty(Uuid7.NewUuid7()))
			.ForMember(dest => dest.FediId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Authority, opt => opt.MapFrom(MapAuthority))
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.Type, opt => opt.Ignore())
			.ForMember(dest => dest.SharedInbox, opt => opt.MapFrom(src => src.SharedInbox))
			.ForMember(dest => dest.OwnedBy, opt => opt.Ignore())
			.ForMember(dest => dest.Accessors, opt => opt.Ignore())
			.ForMember(dest => dest.Audiences, opt => opt.Ignore())
			.ForMember(dest => dest.Headlining, opt => opt.Ignore())
			.ForMember(dest => dest.FollowersCollection, opt => opt.Ignore())
			.ForMember(dest => dest.FollowingCollection, opt => opt.Ignore())
			.ForMember(dest => dest.FediId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Summary))
			.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.Handle, opt => opt.MapFrom(src => src.PreferredUsername))
			// .ForMember(dest => dest.CustomFields, opt => opt.MapFrom(src => src.Attachment))
			.ForMember(dest => dest.CustomFields, opt => opt.Ignore())
			.ForMember(dest => dest.Inbox, opt => opt.MapFrom(src => src.Inbox))
			.ForMember(dest => dest.Outbox, opt => opt.MapFrom(src => src.Outbox))
			.ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Published))
			.ForMember(dest => dest.Updated, opt => opt.MapFrom(src => src.Updated))
			.ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers))
			.ForMember(dest => dest.Following, opt => opt.MapFrom(src => src.Following))
			.ForMember(d => d.Keys, opt => opt.MapFrom(s => s.PublicKeys))
			// .ForMember(dest => dest.Keys, opt => opt.MapFrom<PublicKeyConverter, List<PublicKey>>(src => src.PublicKey!))
			.AfterMap((profileActor, profile) =>
			{
				if (profileActor.Is<PersonActor>()) profile.Type = ActivityActorType.Person;
				else if (profileActor.Is<ApplicationActor>()) profile.Type = ActivityActorType.Application;
				else if (profileActor.Is<GroupActor>()) profile.Type = ActivityActorType.Group;
				else if (profileActor.Is<ServiceActor>()) profile.Type = ActivityActorType.Service;
				else if (profileActor.Is<OrganizationActor>()) profile.Type = ActivityActorType.Organization;
				else profile.Type = ActivityActorType.Unknown;
			});

	}

	private static void ConfigureKeyTypes(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<PublicKey, SigningKey>()
			.ConvertUsing<PublicKeyConverter>();
		cfg.CreateMap<SigningKey, PublicKey>()
			.ConvertUsing<PublicKeyConverter>();
	}

	private static string MapAuthority(ASObject actor, Models.Profile _)
		=> actor.TryGetId(out var id)
			? id.GetAuthority()
			: throw CoreException.MissingData<Models.Profile>("Mapping failure, no id available");

	private static void FromNote(IMapperConfigurationExpression cfg)
	{
		// cfg.CreateMap<NoteObject, Post>(MemberList.Destination)
		cfg.CreateMap<ASObject, Post>(MemberList.Destination)
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.FediId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Authority, opt => opt.MapFrom((_, post) => post.FediId.GetAuthority()))
			.ForMember(dest => dest.Hostname, opt => opt.MapFrom((_, post) => post.FediId.Host))
			.ForMember(dest => dest.Contents,
				opt => opt.MapFrom<ContentResolver, NaturalLanguageString?>(src => src.Content))
			.ForMember(dest => dest.Creators,
				opt => opt.MapFrom<ProfileResolver, LinkableList<ASObject>>(src => src.AttributedTo))
			.ForMember(dest => dest.InReplyTo,
				opt => opt.MapFrom<PostResolver, LinkableList<ASObject>?>(src => src.InReplyTo))
			.ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
			.ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.Published))
			.ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.Published))
			.ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.Updated))
			.ForMember(dest => dest.LastSeenDate, opt => opt.Ignore())
			.ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
			.ForMember(dest => dest.AddressedTo, opt => opt.ConvertUsing<MentionsConverter, ASObject>(src => src))
			.ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Generator))
			.ForMember(dest => dest.LikesCollection,
				opt => opt.MapFrom<ProfileResolver, Linkable<ASCollection>?>(src => src.Likes))
			.ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes))
			.ForMember(dest => dest.SharesCollection,
				opt => opt.MapFrom<ProfileResolver, Linkable<ASCollection>?>(src => src.Shares))
			.ForMember(dest => dest.Shares, opt => opt.MapFrom(src => src.Shares))
			.ForMember(dest => dest.ContentRootIdUri, opt => opt.Ignore())
			.ForMember(dest => dest.Thread,
				opt => opt.MapFrom<PostContextConverter, LinkableList<ASObject>?>(src => src.InReplyTo))
			.ForMember(dest => dest.RepliesCollection,
				opt => opt.MapFrom<PostResolver, ASCollection?>(src => src.Replies))
			.ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
			.ForMember(dest => dest.Audience,
				opt => opt.MapFrom<AudienceResolver, LinkableList<ASObject>>(src => src.Audience));
	}

	private static void FromASType(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<ASType, Models.Profile>()
			.ConvertUsing<ASTypeConverter>();
		cfg.CreateMap<ASType, Models.IFederatedActor>()
			.ConvertUsing<ASTypeConverter>();
	}

	private static void ConfigureBaseTypes(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<ASObject?, Uri?>()
			.ConvertUsing<IdConverter>();

		cfg.CreateMap<Linkable<ASObject>?, Uri?>()
			.ConvertUsing<IdConverter>();

		cfg.CreateMap<Linkable<ASCollection>?, Uri?>()
			.ConvertUsing<IdConverter>();

		cfg.CreateMap<string, ReadOnlyMemory<byte>>()
			.ConvertUsing<PublicKeyConverter>();

		cfg.CreateMap<NaturalLanguageString?, string?>()
			.ConvertUsing<NaturalLanguageStringConverter>();
	}

	private static void ConfigureUriTypes(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<Models.Profile, Uri>(MemberList.None)
			.ConstructUsing(profile => profile.FediId);

		cfg.CreateMap<Uri, ASObject>(MemberList.None)
			.ConstructUsing(uri => new ASObject() { Id = uri.ToString() });

		cfg.CreateMap<Uri, Linkable<ASObject>>(MemberList.None)
			.ConstructUsing(uri => new Linkable<ASObject>(new ASLink() { HRef = uri }));

		cfg.CreateMap<Uri, LinkableList<ASObject>>(MemberList.None)
			.ConstructUsing(uri => new Linkable<ASObject>(new ASLink() { HRef = uri }));

		cfg.CreateMap<IEnumerable<Uri>, LinkableList<ASObject>>(MemberList.None)
			.ConstructUsing((list, maps) => new LinkableList<ASObject>(list.Select(each => maps.Mapper.Map<Linkable<ASObject>>(each))));

		cfg.CreateMap<Audience, Uri>(MemberList.None)
			.ConstructUsing(audience => audience.FediId);
	}

	private static void ConfigureDateTypes(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<DateTime, DateTimeOffset>(MemberList.None);
		cfg.CreateMap<DateTimeOffset, DateTime>(MemberList.None)
			.ConstructUsing(dto => dto.DateTime);
	}
}

internal class MentionsConverter : IValueConverter<ASObject, ICollection<Mention>>
{
	public ICollection<Mention> Convert(ASObject sourceMember, ResolutionContext context)
	{
		var result = new HashSet<Mention>();
		foreach (var l in sourceMember.To)
		{
			result.Add(new Mention(Models.Profile.CreateEmpty(context.Mapper.Map<Uri>(l)), MentionVisibility.To));
		}
		foreach (var l in sourceMember.BTo)
		{
			result.Add(new Mention(Models.Profile.CreateEmpty(context.Mapper.Map<Uri>(l)), MentionVisibility.Bto));
		}
		foreach (var l in sourceMember.CC)
		{
			result.Add(new Mention(Models.Profile.CreateEmpty(context.Mapper.Map<Uri>(l)), MentionVisibility.Cc));
		}
		foreach (var l in sourceMember.BCC)
		{
			result.Add(new Mention(Models.Profile.CreateEmpty(context.Mapper.Map<Uri>(l)), MentionVisibility.Bcc));
		}

		return result;
	}
}

/// <summary>
/// Resolve the Replies collection as part of a Post object
/// </summary>
public class PostRepliesResolver : IMemberValueResolver<Post, NoteObject, Uri?, ASCollection?>
{
	public ASCollection? Resolve(Post source, NoteObject destination, Uri? sourceMember, ASCollection? destMember, ResolutionContext context)
	{
		var result = new ASCollection
		{
			Id = sourceMember?.ToString(),
			First = sourceMember == null ? null : new Linkable<ASCollectionPage>(new ASLink{HRef = sourceMember}),
			TotalItems = source.RepliesCollection.Count,
		};
		return result;
	}
}

[UsedImplicitly]
public class AttachedContentResolver : IMemberValueResolver<Post, NoteObject, IEnumerable<Content>, LinkableList<ASObject>>
{
	public LinkableList<ASObject> Resolve(Post source, NoteObject destination, IEnumerable<Content> sourceMember,
		LinkableList<ASObject> destMember, ResolutionContext context)
	{
		return new LinkableList<ASObject>(sourceMember.Order().Select(context.Mapper.Map<ASObject>));
	}
}

[UsedImplicitly]
public class FediIdResolver : IMemberValueResolver<Post, NoteObject, Post?, LinkableList<ASObject>?>,
	IMemberValueResolver<Content, ASObject, IEnumerable<Models.Profile>, LinkableList<ASObject>>
{
	public LinkableList<ASObject>? Resolve(Post source, NoteObject destination, Post? sourceMember, LinkableList<ASObject>? destMember,
		ResolutionContext context)
	{
		return sourceMember != null ? context.Mapper.Map<LinkableList<ASObject>>(sourceMember.FediId) : default;
	}

	public LinkableList<ASObject> Resolve(Content source, ASObject destination, IEnumerable<Models.Profile> sourceMember,
		LinkableList<ASObject> destMember, ResolutionContext context)
	{
		destMember.AddRange(sourceMember.Select(each => new ASLink { HRef = each.FediId }));
		return destMember;
	}
}

[UsedImplicitly]
public class MediaTypeResolver : IMemberValueResolver<Post, NoteObject, Content?, string?>
{
	public string? Resolve(Post source, NoteObject destination, Content? sourceMember, string? destMember, ResolutionContext context)
	{
		return sourceMember is { } content ? content.ContentType.Name : default;
	}
}

[UsedImplicitly]
internal class ProfileResolver :
	IMemberValueResolver<ASObject, Post, LinkableList<ASObject>, ICollection<Models.Profile>>,
	IMemberValueResolver<ASObject, Post, Linkable<ASCollection>?, IList<Models.Profile>>
{
	public ICollection<Models.Profile> Resolve(ASObject source, Post destination, LinkableList<ASObject> sourceMember,
		ICollection<Models.Profile> destMember,
		ResolutionContext context)
	{
		foreach (var id in sourceMember.ValueItems.SelectIds())
		{
			var profile = Models.Profile.CreateEmpty(id);
			destMember.Add(profile);
		}

		foreach (var id in sourceMember.LinkItems.SelectIds())
		{
			var profile = Models.Profile.CreateEmpty(id);
			destMember.Add(profile);
		}

		return destMember;
	}

	public IList<Models.Profile> Resolve(ASObject source, Post destination, Linkable<ASCollection>? sourceMember,
		IList<Models.Profile> destMember, ResolutionContext context)
	{
		if (sourceMember is null) return destMember;
		if (sourceMember.Value is not { } value) return destMember;
		foreach (var profile in context.Mapper.Map<ICollection<Models.Profile>>(value.Items))
		{
			destMember.Add(profile);
		}

		return destMember;
	}
}

[UsedImplicitly]
internal class PostResolver :
	IMemberValueResolver<ASObject, Post, LinkableList<ASObject>?, Post?>,
	IMemberValueResolver<ASObject, Post, ASCollection?, IList<Post>>
{
	public Post? Resolve(ASObject source, Post destination, LinkableList<ASObject>? sourceMember, Post? destMember,
		ResolutionContext context)
	{
		if (sourceMember is null) return default;
		if (!sourceMember.Any()) return default;
		if (!sourceMember.First().TryGetId(out var id)) return default;

		return new Post(id, destination.Thread);
	}

	public IList<Post> Resolve(ASObject source, Post destination, ASCollection? sourceMember, IList<Post> destMember,
		ResolutionContext context)
	{
		if (sourceMember is null) return destMember;
		if (sourceMember.Items is null) return destMember;

		foreach (var id in sourceMember.Items.ValueItems.SelectIds())
		{
			var post = new Post(fediId: id, thread: destination.Thread);
			destMember.Add(post);
		}

		foreach (var id in sourceMember.Items.LinkItems.SelectIds())
		{
			var post = new Post(fediId: id, thread: destination.Thread);
			destMember.Add(post);
		}

		return destMember;
	}
}

[UsedImplicitly]
internal class AudienceResolver : IMemberValueResolver<ASObject, Post, LinkableList<ASObject>, ICollection<Audience>>
{
	public ICollection<Audience> Resolve(ASObject src, Post post, LinkableList<ASObject> srcAudience,
		ICollection<Audience> postAudience,
		ResolutionContext mappingContext)
	{
		AddToAudience(srcAudience);
		AddToAudience(src.To);
		AddToAudience(src.CC);
		AddToAudience(src.BCC);
		AddToAudience(src.BTo);

		return postAudience;

		void AddToAudience(LinkableList<ASObject> objects)
		{
			foreach (var audience in objects.LinkItems.SelectIds())
			{
				postAudience.Add(Audience.FromUri(audience));
			}

			foreach (var id in objects.ValueItems.SelectIds())
			{
				postAudience.Add(Audience.FromUri(id));
			}
		}
	}
}

[UsedImplicitly]
internal class PostContextConverter : IMemberValueResolver<ASObject, Post, LinkableList<ASObject>?, ThreadContext>
{
	public ThreadContext Resolve(ASObject src, Post post, LinkableList<ASObject>? inReplyTo, ThreadContext thread,
		ResolutionContext mappingContext)
	{
		return inReplyTo?.Any() == false
			? NewThread(src, post)
			: NewReply(src, post, mappingContext);
	}

	private static ThreadContext NewThread(ASObject src, Post post)
	{
		if (src.Context?.TryGetValue(out var ctx) == true
		    && ctx.Is<ASCollection>()
		    && ctx.TryGetId(out var ctxId))
		{
			return Result(ctxId);
		}

		var srcId = src.Replies?.Id ?? src.Id;
		return Result(srcId is null ? null : new Uri(srcId));

		ThreadContext Result(Uri? id)
		{
			var result = new ThreadContext
			{
				FediId = id,
				RootId = post.Id,
				Heuristics = new Heuristics
				{
					NewThread = true
				}
			};
			result.Posts.Add(post);
			return result;
		}
	}

	private static ThreadContext NewReply(ASObject src, Post post, ResolutionContext mappingContext)
	{
		var heuristic = new Heuristics
		{
			NewThread = false
		};

		if (src.Context?.TryGetId(out var id) == true)
			heuristic.Context = id;

		return new ThreadContext
		{
			FediId = new Uri(Extensions.NotNull(src.Replies?.Id, src.Context?.Value?.Id,
				src.Context?.Link?.HRef.ToString(), src.Id)),
			RootId = post.Id,
			Heuristics = heuristic
		};
	}
}

[UsedImplicitly]
internal class
	ContentResolver : IMemberValueResolver<ASObject, Post, NaturalLanguageString?, ICollection<Content>>
{
	ICollection<Content> IMemberValueResolver<ASObject, Post, NaturalLanguageString?, ICollection<Content>>
		.Resolve(ASObject source, Post post, NaturalLanguageString? sourceContent,
			ICollection<Content>? dest, ResolutionContext context)
	{
		if (!source.TryGetId(out var id)) return post.Contents;
		// TODO: use explicit type mappers for more robust mapping
		// like context.Mapper.Map<Note>(noteObject);
		// TODO(content types): map additional content types here
		var content = source.Is<NoteObject>() ? new Note() { FediId = id, Post = post, Html = sourceContent?.DefaultValue ?? ""}
			: default(Content);

		if (content == null) return post.Contents;
		if (source.Preview?.TryGetValue(out var value) == true)
			content.Preview = context.Mapper.Map<string>(value);
		else
			content.GeneratePreview();
		post.AddContent(content);

		return post.Contents;
	}
}

[UsedImplicitly]
internal class IdConverter :
	ITypeConverter<ASLink, Uri>,
	ITypeConverter<ASObject?, Uri?>,
	ITypeConverter<Linkable<ASObject>?, Uri?>,
	ITypeConverter<Linkable<ASCollection>?, Uri?>
{
	public Uri Convert(ASLink source, Uri destination, ResolutionContext context)
	{
		return source.HRef;
	}

	public Uri? Convert(ASObject? source, Uri? destination, ResolutionContext context)
	{
		return source?.Id != null ? new Uri(source.Id) : default;
	}

	public Uri? Convert(Linkable<ASObject>? source, Uri? destination, ResolutionContext context)
	{
		return source?.TryGetId(out var id) == true ? id : default;
	}

	public Uri? Convert(Linkable<ASCollection>? source, Uri? destination, ResolutionContext context)
	{
		return source?.TryGetId(out var id) == true ? id : default;
	}
}

[UsedImplicitly]
internal class PublicKeyConverter :
	ITypeConverter<PublicKey, SigningKey>,
	ITypeConverter<SigningKey, PublicKey>,
	IMemberValueResolver<ProfileActor, Models.Profile, PublicKey?, IList<SigningKey>>,
	ITypeConverter<string, ReadOnlyMemory<byte>>,
	IMemberValueResolver<Models.Profile, ProfileActor, SigningKey?, PublicKey?>
{
	public SigningKey Convert(PublicKey source, SigningKey _, ResolutionContext context)
	{
		// if (source is null) return default;
		using TextReader tr = new StringReader(source.PublicKeyPem);

		var reader = new PemReader(tr);
		var pemObject = reader.ReadObject();
		var alg = pemObject switch
		{
			RsaKeyParameters => SigningKey.KeyFamily.Rsa,
			DsaKeyParameters => SigningKey.KeyFamily.Dsa,
			ECKeyParameters => SigningKey.KeyFamily.EcDsa,
			_ => SigningKey.KeyFamily.Unknown
		};

		var result = SigningKey.CreateEmpty(Uuid7.NewUuid7(), new Uri(source.Id));

		result.FediId = new Uri(source.Id);
		result.Label = "From federation peer";
		result.PublicKey = context.Mapper.Map<ReadOnlyMemory<byte>>(source.PublicKeyPem);
		result.Family = alg;
		result.Created = DateTimeOffset.UtcNow;

		return result;
	}

	public PublicKey Convert(SigningKey source, PublicKey _, ResolutionContext context)
	{
		AsymmetricAlgorithm? algo = source.Family switch
		{
			SigningKey.KeyFamily.Rsa => source.GetRsa(),
			SigningKey.KeyFamily.Dsa => source.GetDsa(),
			SigningKey.KeyFamily.EcDsa => source.GetEcDsa(),
			_ => null
		};

		var result = new PublicKey()
		{
			Id = source.FediId.ToString(),
			Owner = default!,
			PublicKeyPem = algo?.ExportSubjectPublicKeyInfoPem() ?? ""
		};

		return result;
	}

	IList<SigningKey> IMemberValueResolver<ProfileActor, Models.Profile, PublicKey?, IList<SigningKey>>
		.Resolve(ProfileActor source, Models.Profile destination, PublicKey? sourceMember,
			IList<SigningKey>? destMember, ResolutionContext context)
	{
		var key = context.Mapper.Map<SigningKey>(sourceMember);
		destMember ??= new List<SigningKey>();
		if (key is not null) destMember.Add(key);

		return destMember;
	}

	ReadOnlyMemory<byte> ITypeConverter<string, ReadOnlyMemory<byte>>
		.Convert(string source, ReadOnlyMemory<byte> destination, ResolutionContext context)
	{
		var b64 = string.Join('\n',
			source.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Skip(1)
				.SkipLast(1));
		return System.Convert.FromBase64String(b64);
	}

	public PublicKey Resolve(Models.Profile source, ProfileActor destination, SigningKey? sourceMember, PublicKey? destMember,
		ResolutionContext context)
	{
		if (destMember != null)
		{
			context.Mapper.Map(sourceMember, destMember);
			return destMember;
		}

		return context.Mapper.Map<PublicKey>(sourceMember);
	}
}

[UsedImplicitly]
public class NaturalLanguageStringConverter
	: ITypeConverter<NaturalLanguageString?, string?>
{
	public string? Convert(NaturalLanguageString? source, string? destination, ResolutionContext context)
	{
		if (source == null)
		{
			return null;
		}

		return source.DefaultValue;
	}
}

[UsedImplicitly]
internal class ASTypeConverter :
	ITypeConverter<ASType, Models.Profile>,
	ITypeConverter<ASType, Models.IFederatedActor>
{
	public Models.Profile Convert(ASType source, Models.Profile? destination, ResolutionContext context)
		=> Convert<ProfileActor, Models.Profile>(source, destination, context);

	/// <summary>
	/// This implementation allows <see cref="Client.Fetch{T}"/> to be called with IFederatedActor as the type. It will
	/// map to any known actor type and return it as an <see cref="IFederatedActor"/>.
	/// </summary>
	public Models.IFederatedActor Convert(ASType source, Models.IFederatedActor? destination, ResolutionContext context)
	{
		IFederatedActor? result = null;

		result = Convert<ProfileActor, Models.Profile>(source, null, context);

		if (destination is null)
		{
			return result;
		}

		destination.FediId = result.FediId;
		destination.Keys = result.Keys;

		return destination;
	}

	private TFederated Convert<TASType, TFederated>(ASType source, TFederated? destination, ResolutionContext context)
		where TASType : ASType, IASModel<TASType>
		where TFederated : IFederated
	{
		if (!source.Is<TASType>(out var typedSource))
		{
			string sourceTypeName = string.Join(", ", source.Type);
			throw new AutoMapperMappingException(
				$"Object with type [{sourceTypeName}] can not be converted to {typeof(TFederated).FullName}.");
		}

		if (destination is null)
		{
			return context.Mapper.Map<TFederated>(typedSource);
		}

		context.Mapper.Map(typedSource, destination);

		return destination;
	}
}