using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PagAI.UI.Services;

namespace PagAI.UI.Views;

public partial class ShellView : UserControl
{
    private readonly Brush CorAtiva = new SolidColorBrush(Color.FromRgb(37, 99, 235));
    private readonly Brush CorInativa = Brushes.Transparent;

    public ShellView()
    {
        InitializeComponent();

        NavigationService.MainContent = MainContent;

        NavigationService.Navigate(new DashboardView());

        MarcarMenu("Dashboard");
    }

    private void BtnDashboard_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new DashboardView());
        MarcarMenu("Dashboard");
    }

    private void BtnAgenda_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new AgendaCobrancasView());
        MarcarMenu("Agenda");
    }

    private void BtnClientes_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ClientesView());
        MarcarMenu("Clientes");
    }

    private void BtnContratos_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ContratosView());
        MarcarMenu("Contratos");
    }

    private void BtnRelatorios_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new RelatoriosView());
        MarcarMenu("Relatorios");
    }

    private void MarcarMenu(string menu)
    {
        BtnClientes.Background = CorInativa;
        BtnContratos.Background = CorInativa;
        BtnDashboard.Background = CorInativa;
        BtnRelatorios.Background = CorInativa;
        BtnAgenda.Background = CorInativa;


        BtnClientes.Foreground = new SolidColorBrush(Color.FromRgb(203, 213, 225));
        BtnContratos.Foreground = new SolidColorBrush(Color.FromRgb(203, 213, 225));
        BtnDashboard.Foreground = new SolidColorBrush(Color.FromRgb(203,213,225));
        BtnRelatorios.Foreground = new SolidColorBrush(Color.FromRgb(203,213,225));
        BtnAgenda.Foreground = new SolidColorBrush(Color.FromRgb(203,213,225));
        
        switch (menu)
        {
            case "Dashboard":
                BtnDashboard.Background = CorAtiva;
                BtnDashboard.Foreground = Brushes.White;
                break;

            case "Clientes":
                BtnClientes.Background = CorAtiva;
                BtnClientes.Foreground = Brushes.White;
                break;

            case "Contratos":
                BtnContratos.Background = CorAtiva;
                BtnContratos.Foreground = Brushes.White;
                break;
            
            case "Relatorios":
                BtnRelatorios.Background = CorAtiva;
                BtnRelatorios.Foreground = Brushes.White;
                break;
            
            case "Agenda":
                BtnAgenda.Background = CorAtiva;
                BtnAgenda.Foreground = Brushes.White;
                break;    
        }
    }
}