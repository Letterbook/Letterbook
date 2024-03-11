using AutoMapper;
using Letterbook.Api.Mappers;
using Letterbook.Core.Tests;

namespace Letterbook.Api.Tests;

public class MapperTests : WithMocks
{
	private const string postJson = """
	                                {
	                                  "id25": "string",
	                                  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	                                  "fediId": "string",
	                                  "thread": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	                                  "summary": "string",
	                                  "preview": "string",
	                                  "source": "string",
	                                  "creators": [
	                                    "3fa85f64-5717-4562-b3fc-2c963f66afa6"
	                                  ],
	                                  "createdDate": "2024-03-08T05:46:18.301Z",
	                                  "publishedDate": "2024-03-08T05:46:18.301Z",
	                                  "updatedDate": "2024-03-08T05:46:18.301Z",
	                                  "contents": [
	                                    {
	                                      "summary": "string",
	                                      "preview": "string",
	                                      "source": "string",
	                                      "sortKey": 0,
	                                      "type": "string",
	                                      "text": "string"
	                                    }
	                                  ],
	                                  "audience": [
	                                    {
	                                      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	                                      "fediId": "string"
	                                    }
	                                  ],
	                                  "addressedTo": [
	                                    {
	                                      "mentioned": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	                                      "visibility": 0
	                                    }
	                                  ],
	                                  "inReplyTo": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
	                                  "repliesCollection": [
	                                    "3fa85f64-5717-4562-b3fc-2c963f66afa6"
	                                  ],
	                                  "likesCollection": [
	                                    "3fa85f64-5717-4562-b3fc-2c963f66afa6"
	                                  ],
	                                  "sharesCollection": [
	                                    "3fa85f64-5717-4562-b3fc-2c963f66afa6"
	                                  ]
	                                }
	                                """;

	private MappingConfigProvider _mappingConfig;
	private Mapper _mapper;

	public MapperTests()
	{
		_mappingConfig = new MappingConfigProvider(new BaseMappings(CoreOptionsMock));
		_mapper = new Mapper(_mappingConfig.Posts);
	}

	[Fact(DisplayName = "Posts config is valid")]
	public void ValidPosts()
	{
		_mappingConfig.Posts.AssertConfigurationIsValid();
	}
}