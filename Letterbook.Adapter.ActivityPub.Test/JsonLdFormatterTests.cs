using System.Text;
using ActivityPub.Types;
using ActivityPub.Types.AS;
using ActivityPub.Types.AS.Extended.Activity;
using ActivityPub.Types.AS.Extended.Actor;
using ActivityPub.Types.Conversion;
using Letterbook.Adapter.ActivityPub.Test.Fixtures;
using Letterbook.Adapter.ActivityPub.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Letterbook.Adapter.ActivityPub.Test;

public class JsonLdFormatterTests : IClassFixture<JsonLdSerializerFixture>
{
    private JsonLdOutputFormatter _formatter;
    private IJsonLdSerializer _serializer;
    private ServiceCollection _serviceCollection;
    private ASActivity _activity;
    private PersonActorExtension _personActor;

    public JsonLdFormatterTests(JsonLdSerializerFixture fixture)
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.TryAddTypesModule();
        _serviceCollection.AddLogging();
        _serializer = fixture.JsonLdSerializer;
        _formatter = new JsonLdOutputFormatter();

        var person = new PersonActor
        {
            Inbox = "https://example.com/inbox",
            Outbox = "https://example.com/outbox"
        }; 
        _personActor = new PersonActorExtension(person)
        {
            Id = "https://example.com/actor",
            Inbox = person.Inbox,
            Outbox = person.Outbox,
            PublicKey =
                new PublicKey
                {
                    Id = "https://example.com/key",
                    PublicKeyPem = "----begin fake public key----",
                }
        };
        _personActor.PublicKey.Owner = _personActor.Id;
        
        _activity = new FollowActivity();
        _activity.Actor.Add(_personActor);
        
    }

    [Fact]
    public void SerializerExists()
    {
        Assert.NotNull(_serializer);
    }

    [Fact]
    public void FormatterExists()
    {
        Assert.NotNull(_formatter);
    }

    [Fact]
    public async Task FormatActivity()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        httpContext.RequestServices = _serviceCollection.BuildServiceProvider();
        var context = new OutputFormatterWriteContext(
            httpContext,
            (stream, encoding) => new HttpResponseStreamWriter(stream, encoding),
            typeof(ASActivity),
            _activity
        ) {
            ContentType = "application/ld+json"
        };
        
        await _formatter.WriteResponseBodyAsync(context, Encoding.UTF8);
        
        Assert.True(httpContext.Response.Body.Length > 0);

        httpContext.Response.Body.Position = 0;
        using var reader = new StreamReader(httpContext.Response.Body);
        var actual = await reader.ReadToEndAsync();
        Assert.NotEqual("", actual);
    }
    
    [Fact]
    public async Task FormatActorWithExtensions()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        httpContext.RequestServices = _serviceCollection.BuildServiceProvider();
        var context = new OutputFormatterWriteContext(
            httpContext,
            (stream, encoding) => new HttpResponseStreamWriter(stream, encoding),
            typeof(PersonActorExtension),
            _activity
        ) {
            ContentType = "application/ld+json"
        };
        
        await _formatter.WriteResponseBodyAsync(context, Encoding.UTF8);
        
        Assert.True(httpContext.Response.Body.Length > 0);

        httpContext.Response.Body.Position = 0;
        using var reader = new StreamReader(httpContext.Response.Body);
        var actual = await reader.ReadToEndAsync();
        Assert.Contains("----begin fake public key----", actual);
    }

    [Fact]
    public void SerializeActorWithExtensions()
    {
        var actual = _serializer.Serialize(_personActor);
        Assert.NotNull(actual);
        Assert.Contains("----begin fake public key----", actual);
    }

    [Fact]
    public void SerializeActivity()
    {
        var actual = _serializer.Serialize(_activity);
        Assert.NotNull(actual);
        Assert.NotEqual("", actual);
    }
}