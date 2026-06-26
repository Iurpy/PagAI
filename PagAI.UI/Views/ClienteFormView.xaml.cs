using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Services;
using PagAI.Domain.Entities;
using PagAI.UI.Services;
using PagAI.UI.Session;

namespace PagAI.UI.Views;

public partial class ClienteFormView : UserControl
{
    private readonly ClienteService _clienteService;

    public ClienteFormView()
    {
        InitializeComponent();

        _clienteService = App.ServiceProvider.GetRequiredService<ClienteService>();
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var cliente = new Cliente
            {
                Nome = TxtNome.Text,
                Telefone = TxtTelefone.Text,
                CPF = TxtCPF.Text,
                EnderecoRua = TxtEnderecoRua.Text,
                Apelido = TxtApelido.Text,
                Observacao = TxtObservacao.Text,
                UsuarioId = UsuarioSessao.UsuarioId
            };

            await _clienteService.CadastrarAsync(cliente);

            NotificationService.Success("Cliente cadastrado com sucesso!");

            NavigationService.Navigate(new ClientesView());
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ClientesView());
    }
}