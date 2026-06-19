using Application.Ports;
using Application.Slces;
using Crosscutting.Options;
using FluentValidation;
using Host.Models;
using Host.Validators;
using Persistence.Adapters;
using FileOptions = Crosscutting.Options.FileOptions;

namespace Host.Ioc;

public static class DependencyExtensions
{
   public static IServiceCollection AddApplication(this IServiceCollection services)
   {
       return services
           .AddTransient<IAccountRetrievalService, AccountRetrievalService>()
           .AddTransient<IAccountCreationService, AccountCreationService>();
   }

   public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
   {
       return services
           .Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)))
           .Configure<FileOptions>(configuration.GetSection(nameof(FileOptions)));
   }

   public static IServiceCollection AddPersistence(this IServiceCollection services)
   {
       return services
           .AddSingleton<IAccountRepository, FileAccountRepository>();
   }

   public static IServiceCollection AddValidation(this IServiceCollection services)
   {
       return services
           .AddTransient<IValidator<CreateAccountRequest>, CreateAccountRequestValidator>();
   }
}
