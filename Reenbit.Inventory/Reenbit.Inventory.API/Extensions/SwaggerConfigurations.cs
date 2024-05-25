using System;
using System.IO;
using System.Linq;
using Reenbit.Inventory.API.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Reenbit.Inventory.API.Extensions;

public static class SwaggerConfigurations
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, string prefix)
    {
        services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc(RoutingConstants.Documentation._clientInterface,
                new OpenApiInfo
                { Title = RoutingConstants.Documentation._baseApiName, Version = "v1", Description = "" });
            swagger.SwaggerDoc(RoutingConstants.Documentation._technicalInterface,
                new OpenApiInfo
                { Title = RoutingConstants.Documentation._technicalApiName, Version = "v1", Description = "" });

            swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Reenbit.Inventory.API.xml"));

            if (!string.IsNullOrEmpty(prefix))
            {
                swagger.DocumentFilter<PathPrefixInsertDocumentFilter>(prefix);
            }
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(cfg =>
        {
            cfg.DefaultModelsExpandDepth(-1);
            cfg.DocumentTitle = "Reenbit Inventory API";
            cfg.SwaggerEndpoint("client-interface/swagger.json",
                RoutingConstants.Documentation._baseApiName);
            cfg.SwaggerEndpoint("technical-interface/swagger.json",
                RoutingConstants.Documentation._technicalApiName);
        });

        return app;
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PathPrefixInsertDocumentFilter(string prefix) : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.Keys.ToList();
            foreach (var path in paths)
            {
                var pathToChange = swaggerDoc.Paths[path];
                swaggerDoc.Paths.Remove(path);
                swaggerDoc.Paths.Add("/" + prefix + path, pathToChange);
            }
        }
    }
}