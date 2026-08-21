using Host.Extensions;
using Host.Handlers.Endpoints;
using Host.Handlers.ErrorHandlers;
using Host.Ioc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddExceptionHandler<GlobalErrorHandler>()
    .AddProblemDetails()
    .AddJwtAuthentication(builder.Configuration)
    .AddOpenApi()
    .AddApplication()
    .AddPersistence()
    .AddValidation()
    .AddConfiguration(builder.Configuration);


var application = builder.Build();

application
    .UseExceptionHandler()
    .UseAuthentication()
    .UseAuthorization();

application.MapOpenApi();
application.MapScalarApiReference();

application
    .MapEndpoints()
    .Run();
