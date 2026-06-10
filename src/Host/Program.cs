using Host.Extensions;
using Host.Handlers.ErrorHandlers;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddExceptionHandler<GlobalErrorHandler>()
    .AddJwtAuthentication(builder.Configuration);

var application = builder.Build();

application
    .UseAuthentication()
    .UseAuthorization();

application.Run();
