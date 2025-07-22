using Microsoft.OpenApi.Models;

namespace WebAPI.ExtensionMethods;

public static class SwaggerServiceExtensions
{
    public static void ConfigureSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Swagger JWT tanımı
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT token'ı girin. Örnek: Bearer {token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
