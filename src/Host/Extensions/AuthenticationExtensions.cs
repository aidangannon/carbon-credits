using Crosscutting.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Host.Extensions;

public static class ServiceDependencyExtensions
{
   public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
   {
        var settings = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>() ?? throw new ApplicationException($"{nameof(JwtOptions)} is has not been configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = settings.Issuer,
                   ValidAudience = settings.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key))
               };
           });

       return services.AddAuthorization();
   }
}
