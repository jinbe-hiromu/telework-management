
using WorkScheduler.Views;

namespace WorkScheduler;

public partial class AppShell : Shell
{
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    void RegisterRoutes()
    {
        Routes.Add(nameof(SchedulerView), typeof(SchedulerView));
        Routes.Add(nameof(InputDetails), typeof(InputDetails));
        Routes.Add(nameof(MainPage), typeof(MainPage));
        Routes.Add(nameof(HamburgerMenu), typeof(HamburgerMenu));


        foreach (var item in Routes)
        {
            Routing.RegisterRoute(item.Key, item.Value);
        }
    }
}
