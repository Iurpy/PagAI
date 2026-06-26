using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Services;
using PagAI.Domain.Entities;
using PagAI.UI.Services;
using PagAI.UI.Session;

namespace PagAI.UI.Views;

public partial class ContratosView : UserControl
{
    private readonly ContratoService _contratoService;
    private List<Contrato> _contratos = [];

    public ContratosView()
    {
        InitializeComponent();

        _contratoService = App.ServiceProvider.GetRequiredService<ContratoService>();

        CarregarContratos();
    }

    private async void CarregarContratos()
    {
        try
        {
            _contratos = await _contratoService.ListarPorUsuarioAsync(UsuarioSessao.UsuarioId);
            ListaContratos.ItemsSource = _contratos;
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private void BtnNovoContrato_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ContratoWizardView());
    }

    private void BtnDetalhes_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button botao || botao.DataContext is not Contrato contrato)
        {
            NotificationService.Error("Não foi possível abrir os detalhes do contrato.");
            return;
        }

        NavigationService.Navigate(new ContratoDetalhesView(contrato.Id));

    }

    private void TxtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var texto = TxtPesquisar.Text.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(texto))
        {
            ListaContratos.ItemsSource = _contratos;
            return;
        }

        var filtrados = _contratos
            .Where(c =>
                c.Titulo.ToLower().Contains(texto) ||
                c.Cliente.Nome.ToLower().Contains(texto) ||
                c.Status.ToString().ToLower().Contains(texto))
            .ToList();

        ListaContratos.ItemsSource = filtrados;
    }
}