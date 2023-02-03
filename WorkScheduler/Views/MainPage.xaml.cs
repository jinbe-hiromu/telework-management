using WorkScheduler.Models;
using WorkScheduler.Services;

namespace WorkScheduler.Views;

public partial class MainPage : FlyoutPage
{
    private readonly INavigationService _navigation;

    public MainPage(INavigationService navigation)
    {
        InitializeComponent();
        _navigation = navigation;

        Detail = new NavigationPage(_navigation.ResolvePage<SchedulerView>());

        flyoutPage.collectionView.SelectionChanged += OnSelectionChanged;
    }

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is FlyoutPageItem item)
        {
            Detail = new NavigationPage(_navigation.ResolvePage(item.TargetType));
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }
    }
}