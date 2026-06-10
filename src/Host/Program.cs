using Host.Extensions;
using Host.Handlers.Endpoints;
using Host.Handlers.ErrorHandlers;
using Host.Swagger;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddExceptionHandler<GlobalErrorHandler>()
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerGen(s => s.ExampleFilters())
    .AddSwaggerExamplesFromAssemblyOf<AccountResponseExample>();


var application = builder.Build();

application
    .UseAuthentication()
    .UseAuthorization()
    .UseSwagger()
    .UseSwaggerUI();

application
    .MapEndpoints()
    .Run();
