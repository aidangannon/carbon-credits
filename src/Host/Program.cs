using Host.Extensions;
using Host.Handlers.Endpoints;
using Host.Handlers.ErrorHandlers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddExceptionHandler<GlobalErrorHandler>()
    .AddJwtAuthentication(builder.Configuration)
    .AddOpenApi();


var application = builder.Build();

application
    .UseAuthentication()
    .UseAuthorization();

application.MapOpenApi();
application.MapScalarApiReference();

application
    .MapEndpoints()
    .Run();
