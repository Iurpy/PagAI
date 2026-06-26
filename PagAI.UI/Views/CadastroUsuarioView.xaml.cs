using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Services;
using PagAI.UI.Services;

namespace PagAI.UI.Views;

public partial class CadastroUsuarioView : UserControl
{
    private readonly UsuarioService _usuarioService;

    public CadastroUsuarioView()
    {
        InitializeComponent();
        _usuarioService = App.ServiceProvider.GetRequiredService<UsuarioService>();
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (TxtSenha.Password != TxtConfirmarSenha.Password)
            {
                NotificationService.Warning("As senhas não coincidem.");
                return;
            }

            await _usuarioService.CadastrarAsync(
                TxtNome.Text,
                TxtLogin.Text,
                TxtSenha.Password
            );

            NotificationService.Success("Usuário cadastrado com sucesso!");

            ((MainWindow)System.Windows.Application.Current.MainWindow!)
                .MainContent.Content = new LoginView();
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e)
    {
        ((MainWindow)System.Windows.Application.Current.MainWindow!)
            .MainContent.Content = new LoginView();
    }
}