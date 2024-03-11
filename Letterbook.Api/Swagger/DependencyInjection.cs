using Letterbook.Api.Dto;
using Medo;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Letterbook.Api.Swagger;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        return services.AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
        {
            options.SwaggerDoc(Docs.LetterbookV1Desc.Name,
                new OpenApiInfo
                {
                    Title = "Letterbook APIs",
                    Description = "Letterbook's first party API",
                    Version = "v1",
                    Contact = new() { Url = new Uri("https://letterbookhq.com") }
                });
            options.SwaggerDoc(Docs.MastodonV1Desc.Name,
                new OpenApiInfo
                {
                    Title = "Mastodon APIs",
                    Description = "Letterbook's implementation of the Mastodon APIs",
                    Version = "v1",
                    Contact = new() { Url = new Uri("https://docs.joinmastodon.com") }
                });
            options.SwaggerDoc(Docs.ActivityPubV1Desc.Name,
                new OpenApiInfo
                {
                    Title = "ActivityPub endpoints",
                    Version = "v1",
                    Description = "ActivityPub objects and specified endpoints",
                    Contact = new() { Url = new Uri("https://www.w3.org/TR/activitypub/") }
                });
            options.MapType<Uuid7>(() => new OpenApiSchema{Type = "string", Format = "uuid"});
            options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Authorization header, including the Bearer scheme, like so: `Bearer <JWT>`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
            options.OperationFilter<RequiredHeaders>();
        });
    }

    public static void UseSwaggerConfig(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(opts =>
        {
            opts.ConfigObject.Urls = new List<UrlDescriptor>()
            {
                Docs.LetterbookV1Desc,
                Docs.MastodonV1Desc,
                Docs.ActivityPubV1Desc
            };
        });
    }
}