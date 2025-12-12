using AutoMapper;
using Medo;

namespace Letterbook.Web.Mappers;

public class FormMappings : AutoMapper.Profile
{
	public FormMappings()
	{
		CreateMap<Forms.PostEditorFormData, Models.Post>(MemberList.Source)
			.ConstructUsing(formData => new Models.Post { Id = formData.Id ?? new Models.PostId(Uuid7.NewUuid7()) })
			.ForMember(post => post.Creators, opt => opt.MapFrom(data => data.Authors))
			.ForMember(post => post.Audience, opt => opt.MapFrom(data => data.Audience.Keys.Select(k => new Uri(k))))
			.ForMember(post => post.Contents,
				opt => opt.MapFrom((data, post, contents, ctx) =>
				{
					contents.Add(ctx.Mapper.Map<Models.Content>(data.Note));
					return contents;
				} ))
			// .ForMember(post => post.Contents, opt => opt.Ignore())
			.ForSourceMember(data => data.Note, opt => opt.DoNotValidate())
			.ForSourceMember(data => data.Audience, opt => opt.DoNotValidate());

		CreateMap<Forms.PostEditorContentData, Models.Note>(MemberList.Source)
			.ForMember(note => note.SourceText, opt => opt.MapFrom(dto => dto.Contents))
			.ForMember(note => note.SourceContentType, opt => opt.MapFrom((dto) => Models.Content.PlainTextMediaType))
			.ForMember(note => note.Html, opt => opt.MapFrom(dto => dto.Contents));
			// .ForMember(note => note.Id, opt => opt.MapFrom(dto => dto.Id))
			// .ForMember(note => note.Id, opt => opt.MapFrom(data => Guid.Empty));

		CreateMap<Forms.PostEditorContentData, Models.Content>(MemberList.None)
			.ConstructUsing((formData, ctx) => ctx.Mapper.Map<Models.Note>(formData))
			.AfterMap((_, ct) =>
			{
				ct.GeneratePreview();

				// TODO: do this in the calling code
				// ct.SetLocalFediId(options.Value);
			});
	}
}