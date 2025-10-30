using Microsoft.Extensions.DependencyInjection;

namespace MSJD.FormSaver;

public static class ServiceCollectionExtension
{
    public static void AddFormSaver(this IServiceCollection services)
    {
        services.AddScoped<IFormSaver, FormSaver>();
    }
}
