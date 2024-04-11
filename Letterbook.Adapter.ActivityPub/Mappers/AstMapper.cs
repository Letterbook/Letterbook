using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Collection;
using ActivityPub.Types.AS.Extended.Actor;
using ActivityPub.Types.AS.Extended.Object;
using ActivityPub.Types.Util;
using AutoMapper;
using JetBrains.Annotations;
using Letterbook.Adapter.ActivityPub.Types;
using Letterbook.Core.Models;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace Letterbook.Adapter.ActivityPub.Mappers;

/// <summary>
/// Map ActivityPubSharp objects to Model types
/// </summary>
public static class AstMapper
{
	public static MapperConfiguration Default = new(cfg =>
	{
		ConfigureBaseTypes(cfg);
		FromActor(cfg);
		FromNote(cfg);
		FromASType(cfg);
	});

	private static void FromActor(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<PersonActorExtension, Models.Profile>(MemberList.Destination)
			.ForMember(dest => dest.FediId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Authority, opt => opt.Ignore())
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.Type, opt => opt.Ignore())
			.ForMember(dest => dest.SharedInbox, opt => opt.Ignore())
			.ForMember(dest => dest.OwnedBy, opt => opt.Ignore())
			.ForMember(dest => dest.Accessors, opt => opt.Ignore())
			.ForMember(dest => dest.Audiences, opt => opt.Ignore())
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
			.ForMember(dest => dest.Updated, opt => opt.MapFrom(src => src.Updated))
			.ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers))
			.ForMember(dest => dest.Following, opt => opt.MapFrom(src => src.Following))
			.ForMember(dest => dest.Keys, opt => opt.MapFrom<PublicKeyConverter, PublicKey?>(src => src.PublicKey!))
			.AfterMap((_, profile) => { profile.Type = ActivityActorType.Person; });

		cfg.CreateMap<ApplicationActorExtension, InstanceActor>(MemberList.Destination)
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.FediId, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Authority, opt => opt.Ignore())
			.ForMember(dest => dest.Keys, opt => opt.MapFrom<PublicKeyConverter, PublicKey?>(src => src.PublicKey!));

		cfg.CreateMap<PublicKey?, SigningKey?>()
			.ConvertUsing<PublicKeyConverter>();
	}

	private static void FromNote(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<NoteObject, Post>(MemberList.Destination)
			.ForMember(dest => dest.Id, opt => opt.Ignore())
			.ForMember(dest => dest.Authority, opt => opt.Ignore())
			.ForMember(dest => dest.Hostname, opt => opt.Ignore())
			.ForMember(dest => dest.Contents,
				opt => opt.MapFrom<NoteContentResolver, NaturalLanguageString?>(src => src.Content))
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
			.ForMember(dest => dest.AddressedTo,
				opt => opt.MapFrom<MentionsResolver<To>, LinkableList<ASObject>>(src => src.To))
			.ForMember(dest => dest.AddressedTo,
				opt => opt.MapFrom<MentionsResolver<Cc>, LinkableList<ASObject>>(src => src.CC))
			.ForMember(dest => dest.AddressedTo,
				opt => opt.MapFrom<MentionsResolver<BTo>, LinkableList<ASObject>>(src => src.BTo))
			.ForMember(dest => dest.AddressedTo,
				opt => opt.MapFrom<MentionsResolver<Bcc>, LinkableList<ASObject>>(src => src.BCC))
			.ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Generator))
			.ForMember(dest => dest.LikesCollection,
				opt => opt.MapFrom<ProfileResolver, Linkable<ASCollection>?>(src => src.Likes))
			.ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes))
			.ForMember(dest => dest.SharesCollection,
				opt => opt.MapFrom<ProfileResolver, Linkable<ASCollection>?>(src => src.Shares))
			.ForMember(dest => dest.Shares, opt => opt.MapFrom(src => src.Shares))
			.ForMember(dest => dest.ContentRootIdUri, opt => opt.Ignore())
			.ForMember(dest => dest.FediId, opt => opt.MapFrom(src => src.Id))
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
	    cfg.CreateMap<ASType, Models.InstanceActor>()
		    .ConvertUsing<ASTypeConverter>();
	    cfg.CreateMap<ASType, Models.IFederatedActor>()
		    .ConvertUsing<ASTypeConverter>();
    }

	private static void ConfigureBaseTypes(IMapperConfigurationExpression cfg)
	{
		cfg.CreateMap<ASLink, Uri>()
			.ConvertUsing<IdConverter>();

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
}

[UsedImplicitly]
internal class ProfileResolver :
	IMemberValueResolver<NoteObject, Post, LinkableList<ASObject>, ICollection<Models.Profile>>,
	IMemberValueResolver<NoteObject, Post, Linkable<ASCollection>?, IList<Models.Profile>>
{
	public ICollection<Models.Profile> Resolve(NoteObject source, Post destination, LinkableList<ASObject> sourceMember,
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

	public IList<Models.Profile> Resolve(NoteObject source, Post destination, Linkable<ASCollection>? sourceMember,
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
	IMemberValueResolver<NoteObject, Post, ASCollection?, IList<Post>>
{
	public Post? Resolve(ASObject source, Post destination, LinkableList<ASObject>? sourceMember, Post? destMember,
		ResolutionContext context)
	{
		if (sourceMember is null) return default;
		if (!sourceMember.First().TryGetId(out var id)) return default;

		return new Post(id, destination.Thread);
	}

	public IList<Post> Resolve(NoteObject source, Post destination, ASCollection? sourceMember, IList<Post> destMember,
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
internal class AudienceResolver : IMemberValueResolver<NoteObject, Post, LinkableList<ASObject>, ICollection<Audience>>
{
	public ICollection<Audience> Resolve(NoteObject src, Post post, LinkableList<ASObject> srcAudience,
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

		return Result(new Uri(src.Replies?.Id ?? src.Id!));

		ThreadContext Result(Uri id)
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

		if (src.Context?.TryGetValue(out var ctx) == true && ctx.Is<ASCollection>(out var ctxCollection))
			heuristic.Context = ctxCollection.Id is not null ? new Uri(ctxCollection.Id) : null;

		if (src.Context?.TryGetLink(out var link) == true && link.Is<ASLink>(out var ctxLink))
			heuristic.Root = ctxLink.HRef;

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
	NoteContentResolver : IMemberValueResolver<NoteObject, Post, NaturalLanguageString?, ICollection<Content>>
{
	ICollection<Content> IMemberValueResolver<NoteObject, Post, NaturalLanguageString?, ICollection<Content>>
		.Resolve(NoteObject source, Post post, NaturalLanguageString? sourceContent,
			ICollection<Content>? dest, ResolutionContext context)
	{
		if (!source.TryGetId(out var id)) return post.Contents;
		var note = new Note
		{
			FediId = id,
			Post = post,
			// TODO: multiple languages
			// or even just single languages, but with knowledge of what language is specified
			Text = sourceContent?.DefaultValue ?? ""
		};
		if (source.Preview?.TryGetValue(out var value) == true)
			note.Preview = context.Mapper.Map<string>(value);
		else
			note.GeneratePreview();
		post.AddContent(note);

		return post.Contents;
	}
}

internal interface IMentionDiscriminator
{
	public MentionVisibility Value();
}
internal class To : IMentionDiscriminator
{
	public MentionVisibility Value() => MentionVisibility.To;
}
internal class Cc : IMentionDiscriminator
{
	public MentionVisibility Value() => MentionVisibility.Cc;
}
internal class BTo : IMentionDiscriminator
{
	public MentionVisibility Value() => MentionVisibility.Bto;
}
internal class Bcc : IMentionDiscriminator
{
	public MentionVisibility Value() => MentionVisibility.Bcc;
}


[UsedImplicitly]
internal class MentionsResolver<T> : IMemberValueResolver<ASObject, Post, LinkableList<ASObject>, ICollection<Mention>>
	where T : class, IMentionDiscriminator
{
	private readonly T _visibility = Activator.CreateInstance<T>();

	public ICollection<Mention> Resolve(ASObject src, Post dest, LinkableList<ASObject> srcMember,
		ICollection<Mention> destMember, ResolutionContext context)
	{
		foreach (var id in srcMember.ValueItems.SelectIds().Concat(srcMember.LinkItems.SelectIds()))
		{
			destMember.Add(new Mention(Models.Profile.CreateEmpty(id), _visibility.Value()));
		}

		return destMember;
	}
}

[UsedImplicitly]
internal class IdConverter : ITypeConverter<ASLink, Uri>,
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
	ITypeConverter<PublicKey?, SigningKey?>,
	IMemberValueResolver<PersonActorExtension, Models.Profile, PublicKey?, IList<SigningKey>>,
	IMemberValueResolver<ApplicationActorExtension, Models.InstanceActor, PublicKey?, IList<SigningKey>>,
	ITypeConverter<string, ReadOnlyMemory<byte>>
{
	public SigningKey? Convert(PublicKey? source, SigningKey? destination, ResolutionContext context)
	{
		if (source is null) return default;
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

		destination ??= new SigningKey() { FediId = new Uri(source.Id) };

		destination.FediId = new Uri(source.Id);
		destination.Label = "From federation peer";
		destination.PublicKey = context.Mapper.Map<ReadOnlyMemory<byte>>(source.PublicKeyPem);
		destination.Family = alg;
		destination.Created = DateTimeOffset.Now;

		return destination;
	}

	IList<SigningKey> IMemberValueResolver<PersonActorExtension, Models.Profile, PublicKey?, IList<SigningKey>>
		.Resolve(PersonActorExtension source, Models.Profile destination, PublicKey? sourceMember,
			IList<SigningKey>? destMember, ResolutionContext context)
	{
		var key = context.Mapper.Map<SigningKey>(sourceMember);
		destMember ??= new List<SigningKey>();
		if (key is not null) destMember.Add(key);

		return destMember;
	}

	IList<SigningKey> IMemberValueResolver<ApplicationActorExtension, Models.InstanceActor, PublicKey?, IList<SigningKey>>
		.Resolve(ApplicationActorExtension source, Models.InstanceActor destination, PublicKey? sourceMember,
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
	ITypeConverter<ASType, Models.InstanceActor>,
	ITypeConverter<ASType, Models.IFederatedActor>
{
	public Models.Profile Convert(ASType source, Models.Profile? destination, ResolutionContext context)
		=> Convert<PersonActorExtension, Models.Profile>(source, destination, context);

	public Models.InstanceActor Convert(ASType source, Models.InstanceActor? destination, ResolutionContext context)
		=> Convert<ApplicationActorExtension, Models.InstanceActor>(source, destination, context);

	/// <summary>
	/// This implementation allows <see cref="Client.Fetch{T}"/> to be called with IFederatedActor as the type. It will
	/// map to any known actor type and return it as an <see cref="IFederatedActor"/>.
	/// </summary>
	public Models.IFederatedActor Convert(ASType source, Models.IFederatedActor? destination, ResolutionContext context)
	{
		IFederatedActor? result = null;
		if (source.Is<ApplicationActorExtension>())
		{
			result = Convert<ApplicationActorExtension, InstanceActor>(source, null, context);
		}
		else if (source.Is<PersonActorExtension>())
		{
			result = Convert<PersonActorExtension, Models.Profile>(source, null, context);
		}

		if (result is null)
		{
			return null!;
		}

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
