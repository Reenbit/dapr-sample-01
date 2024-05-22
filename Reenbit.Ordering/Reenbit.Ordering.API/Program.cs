using System;
using Azure.Identity;
using Reenbit.Ordering.API.Controllers;
using Reenbit.Ordering.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Newtonsoft.Json.Converters;
using Reenbit.Ordering.Cqrs.Commands;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(
    typeof(Program).FullName, LogLevel.Warning);

builder.Host.UseDefaultServiceProvider(opt => opt.ValidateScopes = false);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

builder.Services.AddDaprClient();

builder.Services.AddMediatR(
    typeof(OrdersController).Assembly,
    typeof(PlaceOrderCommand).Assembly
);

builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        b => b.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors("CorsPolicy");
}

app.UseSwaggerDocumentation();

app.UseStaticFiles();

app.UseRouting();
app.UseCloudEvents();

app.MapGet("/", context => context.Response.WriteAsync("Ordering Service"))
    .WithMetadata(new AllowAnonymousAttribute());

app.MapControllers();

await app.RunAsync();