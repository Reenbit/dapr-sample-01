using Reenbit.Inventory.API.Controllers;
using Reenbit.Inventory.API.Extensions;
using Reenbit.Inventory.Cqrs.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(
    typeof(Program).FullName, LogLevel.Warning);

builder.Host.UseDefaultServiceProvider(opt => opt.ValidateScopes = false);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers();

builder.Services.AddDaprClient();

builder.Services.AddMediatR(
    typeof(ProductsController).Assembly,
    typeof(GetAllProductsQuery).Assembly
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

app.MapGet("/", context => context.Response.WriteAsync("Inventory Service"))
    .WithMetadata(new AllowAnonymousAttribute());
app.MapControllers();
app.MapSubscribeHandler();

await app.RunAsync();