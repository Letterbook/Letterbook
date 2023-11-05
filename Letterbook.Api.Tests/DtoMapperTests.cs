﻿using AutoMapper;
using Letterbook.Api.Mappers;
using Letterbook.Core.Models;

namespace Letterbook.Api.Tests;

public class DtoMapperTests
{
    private IMapper _mapper;
    
    public DtoMapperTests()
    {
        _mapper = new Mapper(DtoMapper.Config);
    }
    
    [Fact]
    public void ConfigIsValid()
    {
        var config = DtoMapper.Config;
        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void CanMapSimpleNote()
    {
        var dto = new AsAp.Object()
        {
            Id = "https://mastodon.example/note/1234",
            Type = "Note",
            Content = "Some test content",
        };
        dto.AttributedTo.Add(new AsAp.Link("https://letterbook.example/@testuser"));
        var actual = _mapper.Map<Note>(dto);
            
        Assert.NotNull(actual);
        Assert.Equal("Some test content", actual.Content);
        Assert.Equal("https://letterbook.example/@testuser", actual.Creators.First().Id.ToString());
    }
}