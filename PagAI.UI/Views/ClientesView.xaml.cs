using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Services;
using PagAI.Domain.Entities;
using PagAI.UI.Services;
using PagAI.UI.Session;

namespace PagAI.UI.Views;

public partial class ClientesView : UserControl
{
    private readonly ClienteService _clienteService;
    private List<Cliente> _clientes = [];

    public ClientesView()
    {
        InitializeComponent();

        _clienteService = App.ServiceProvider.GetRequiredService<ClienteService>();

        CarregarClientes();
    }

    private async void CarregarClientes()
    {
        _clientes = await _clienteService.ListarPorUsuarioAsync(UsuarioSessao.UsuarioId);
        ListaClientes.ItemsSource = _clientes;
    }

    private void BtnNovoCliente_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ClienteFormView());
    }

    private void TxtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
    {
        var texto = TxtPesquisar.Text.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(texto))
        {
            ListaClientes.ItemsSource = _clientes;
            return;
        }

        var filtrados = _clientes
            .Where(c =>
                c.Nome.ToLower().Contains(texto) ||
                (!string.IsNullOrWhiteSpace(c.Telefone) && c.Telefone.Contains(texto)) ||
                (!string.IsNullOrWhiteSpace(c.CPF) && c.CPF.Contains(texto)))
            .ToList();

        ListaClientes.ItemsSource = filtrados;
    }
}