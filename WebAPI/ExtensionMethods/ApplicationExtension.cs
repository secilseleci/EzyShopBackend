namespace WebAPI.ExtensionMethods;

public static class ApplicationExtension
{
    public static void ConfigureLocalization(this WebApplication app)
    {
        app.UseRequestLocalization(options =>
        {
            options.AddSupportedCultures("en-US")
                .AddSupportedUICultures("en-US")
                .SetDefaultCulture("en-US");
        });
    }
}

