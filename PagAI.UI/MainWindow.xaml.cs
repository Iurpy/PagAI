using System.Windows;
using System.Windows.Input;
using PagAI.UI.Views;

namespace PagAI.UI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainContent.Content = new LoginView();
    }

    private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            AlternarMaximizar();
            return;
        }

        DragMove();
    }

    private void Minimizar_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Maximizar_Click(object sender, RoutedEventArgs e)
    {
        AlternarMaximizar();
    }

    private void Fechar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void AlternarMaximizar()
    {
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }
}