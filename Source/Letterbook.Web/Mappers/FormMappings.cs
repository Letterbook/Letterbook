using AutoMapper;
using Letterbook.Core;
using Medo;
using Microsoft.Extensions.Options;

namespace Letterbook.Web.Mappers;

public class FormMappings : AutoMapper.Profile
{
	public FormMappings(IOptions<CoreOptions> options)
	{
		CreateMap<Forms.PostEditorFormData, Models.Post>(MemberList.Source)
			.ConstructUsing(formData => new Models.Post(options.Value))
			.ForMember(post => post.Id, opt => opt.Condition(dto => dto.Id != Uuid7.Empty))
			.ForMember(post => post.Creators, opt => opt.MapFrom(data => data.Authors))
			.ForMember(post => post.Audience, opt => opt.MapFrom(data => data.Audience.Keys.Select(k => new Uri(k))))
			.ForMember(post => post.Contents,
				opt => opt.MapFrom((data, post, contents, ctx) =>
				{
					contents.Add(ctx.Mapper.Map<Models.Content>(data.Note));
					return contents;
				})
			)
			.ForSourceMember(data => data.Note, opt => opt.DoNotValidate())
			.ForSourceMember(data => data.Audience, opt => opt.DoNotValidate())
			.AfterMap((_, post) =>
			{
				post.SetUris(options.Value);
			});

		CreateMap<Forms.PostEditorContentData, Models.Note>(MemberList.Source)
			.ForMember(note => note.Id, opt => opt.Condition(form => form.Id != Guid.Empty))
			.ForMember(note => note.SourceText, opt => opt.MapFrom(dto => dto.Contents))
			.ForMember(note => note.SourceContentType, opt => opt.MapFrom((dto) => Models.Content.PlainTextMediaType))
			.ForMember(note => note.Html, opt => opt.MapFrom(dto => dto.Contents));

		CreateMap<Forms.PostEditorContentData, Models.Content>(MemberList.None)
			.ConstructUsing((formData, ctx) => ctx.Mapper.Map<Models.Note>(formData)) // switch on other future types
			.ForMember(post => post.Id, opt => opt.Condition(form => form.Id != Guid.Empty))
			.AfterMap((_, ct) =>
			{
				ct.GeneratePreview();
				ct.SetLocalFediId(options.Value);
			});
	}
}