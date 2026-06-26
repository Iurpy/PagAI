using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PagAI.UI.Services;

public static class NavigationService
{
    public static ContentControl? MainContent { get; set; }

    public static async void Navigate(UserControl page)
    {
        if (MainContent is null)
            return;

        if (MainContent.Content is null)
        {
            MainContent.Content = page;
            return;
        }

        var fadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(120),
            EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseInOut
            }
        };

        MainContent.BeginAnimation(UIElement.OpacityProperty, fadeOut);

        await Task.Delay(120);

        MainContent.Content = page;

        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(160),
            EasingFunction = new CubicEase
            {
                EasingMode = EasingMode.EaseInOut
            }
        };

        MainContent.BeginAnimation(UIElement.OpacityProperty, fadeIn);
    }
}