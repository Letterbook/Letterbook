using AutoMapper;
using Medo;

namespace Letterbook.Web.Mappers;

public class FormMappings : AutoMapper.Profile
{
	public FormMappings()
	{
		CreateMap<Forms.PostEditorFormData, Models.Post>()
			.ConstructUsing(formData => new Models.Post { Id = formData.Id ?? new Models.PostId(Uuid7.NewUuid7()) })
			.ForMember(post => post.Creators, opt => opt.MapFrom(data => data.Authors))
			.ForMember(post => post.Audience, opt => opt.MapFrom(data => data.Audience.Keys.Select(k => new Uri(k))))
			.ForMember(post => post.Contents,
				opt => opt.MapFrom((data, post, contents, ctx) => new List<Models.Content> { ctx.Mapper.Map<Models.Content>(data) }));

		CreateMap<Forms.PostEditorFormData, Models.Note>(MemberList.Source)
			.ForMember(note => note.SourceText, opt => opt.MapFrom(dto => dto.Contents))
			.ForMember(note => note.SourceContentType, opt => opt.MapFrom((dto) => Models.Content.PlainTextMediaType))
			.ForMember(note => note.Html, opt => opt.MapFrom(dto => dto.Contents));

		CreateMap<Forms.PostEditorFormData, Models.Content>(MemberList.None)
			.ConstructUsing((formData, ctx) => ctx.Mapper.Map<Models.Note>(formData))
			.AfterMap((_, ct) =>
			{
				ct.GeneratePreview();

				// TODO: do this in the calling code
				// ct.SetLocalFediId(options.Value);
			});
	}
}