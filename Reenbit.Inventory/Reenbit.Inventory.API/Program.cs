using System;
using Azure.Identity;
using Reenbit.Inventory.API.Controllers;
using Reenbit.Inventory.API.Extensions;
using Reenbit.Inventory.Cqrs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Reenbit.Inventory.Cqrs.Actors;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(
    typeof(Program).FullName, LogLevel.Warning);

builder.Host.UseDefaultServiceProvider(opt => opt.ValidateScopes = false);

builder.Host.ConfigureServices(services =>
{
    services.AddApplicationInsightsTelemetry();

    services.AddControllers();

    services.AddDaprClient();

    services.AddMediatR(
        typeof(ProductsController).Assembly,
        typeof(GetAllProductsQuery).Assembly
    );
    
    services.AddHttpContextAccessor();

    services.AddSwaggerDocumentation();

    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            b => b.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
    });

    services.AddActors(options =>
    {
        options.Actors.RegisterActor<ProductActor>();
    });
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("CorsPolicy");
}

app.UseSwaggerDocumentation();

app.UseStaticFiles();

var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

app.UseRouting();
app.UseCloudEvents();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", context => context.Response.WriteAsync("Inventory Service"))
        .WithMetadata(new AllowAnonymousAttribute());
    endpoints.MapControllers();
    endpoints.MapSubscribeHandler();
    endpoints.MapActorsHandlers();
});

await app.RunAsync();