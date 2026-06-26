using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using PagAI.UI.Services;

namespace PagAI.UI.Components;

public partial class ToastHost : UserControl
{
    private readonly DispatcherTimer _timer;

    public ToastHost()
    {
        InitializeComponent();

        ToastService.OnShow += MostrarToast;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3)
        };

        _timer.Tick += (_, _) => EsconderToast();
    }

    private void MostrarToast(string message, ToastType type)
    {
        TxtMessage.Text = message;

        switch (type)
        {
            case ToastType.Success:
                TxtTitle.Text = "Sucesso";
                TxtIcon.Text = "✓";
                IconCircle.Background = new SolidColorBrush(Color.FromRgb(34, 197, 94));
                ProgressBar.Background = new SolidColorBrush(Color.FromRgb(34, 197, 94));
                break;

            case ToastType.Error:
                TxtTitle.Text = "Erro";
                TxtIcon.Text = "!";
                IconCircle.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                ProgressBar.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68));
                break;

            case ToastType.Warning:
                TxtTitle.Text = "Atenção";
                TxtIcon.Text = "!";
                IconCircle.Background = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                ProgressBar.Background = new SolidColorBrush(Color.FromRgb(245, 158, 11));
                break;

            default:
                TxtTitle.Text = "Informação";
                TxtIcon.Text = "i";
                IconCircle.Background = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                ProgressBar.Background = new SolidColorBrush(Color.FromRgb(37, 99, 235));
                break;
        }

        _timer.Stop();

        ToastContainer.Opacity = 0;
        ToastTransform.X = 40;
        ProgressBar.Width = 358;

        var storyboard = new Storyboard();

        var fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromMilliseconds(220),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(fadeIn, ToastContainer);
        Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));

        var slideIn = new DoubleAnimation
        {
            From = 40,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(220),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(slideIn, ToastTransform);
        Storyboard.SetTargetProperty(slideIn, new PropertyPath(TranslateTransform.XProperty));

        var progress = new DoubleAnimation
        {
            From = 358,
            To = 0,
            Duration = TimeSpan.FromSeconds(3)
        };

        Storyboard.SetTarget(progress, ProgressBar);
        Storyboard.SetTargetProperty(progress, new PropertyPath(WidthProperty));

        storyboard.Children.Add(fadeIn);
        storyboard.Children.Add(slideIn);
        storyboard.Children.Add(progress);

        storyboard.Begin();

        _timer.Start();
    }

    private void EsconderToast()
    {
        _timer.Stop();

        var storyboard = new Storyboard();

        var fadeOut = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(180),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };

        Storyboard.SetTarget(fadeOut, ToastContainer);
        Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));

        var slideOut = new DoubleAnimation
        {
            From = 0,
            To = 40,
            Duration = TimeSpan.FromMilliseconds(180),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };

        Storyboard.SetTarget(slideOut, ToastTransform);
        Storyboard.SetTargetProperty(slideOut, new PropertyPath(TranslateTransform.XProperty));

        storyboard.Children.Add(fadeOut);
        storyboard.Children.Add(slideOut);

        storyboard.Begin();
    }
}