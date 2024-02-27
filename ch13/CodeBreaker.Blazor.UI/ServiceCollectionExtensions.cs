using Microsoft.Extensions.DependencyInjection;

namespace CodeBreaker.Blazor.UI;
public static class ServiceCollectionExtensions
{
    public static void AddCodeBreakerUI(this IServiceCollection services)
    {
        //services.AddScoped<ICodeBreakerDialogService, CodeBreakerDialogService>();
        //services.AddScoped<IThemeService<float>, ThemeService>();
    }
}
