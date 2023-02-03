using System.Net;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using WorkScheduler.Services;
using WorkScheduler.ViewModels;
using WorkScheduler.Views;

namespace WorkScheduler;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.ConfigureSyncfusionCore();
        builder
            .UseMauiCommunityToolkit()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<LoginView>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<SchedulerView>();
        builder.Services.AddTransient<SchedulerViewModel>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DataQueryView>();
        builder.Services.AddTransient<DataQueryViewModel>();
        builder.Services.AddSingleton(new CookieContainer());
        builder.Services.AddSingleton<INavigationService, NavigationService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
