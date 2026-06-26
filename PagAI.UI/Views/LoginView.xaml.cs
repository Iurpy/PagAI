using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Services;
using PagAI.UI.Services;
using PagAI.UI.Session;

namespace PagAI.UI.Views;

public partial class LoginView : UserControl
{
    private readonly UsuarioService _usuarioService;

    public LoginView()
    {
        InitializeComponent();
        _usuarioService = App.ServiceProvider.GetRequiredService<UsuarioService>();
    }

    private async void BtnEntrar_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var usuario = await _usuarioService.LoginAsync(
                TxtLogin.Text,
                TxtSenha.Password);

            UsuarioSessao.UsuarioLogado = usuario;

            NotificationService.Success($"Bem-vindo, {usuario.Nome}!");

            var mainWindow = Window.GetWindow(this) as MainWindow;

            if (mainWindow == null)
                return;

            mainWindow.MainContent.Content = new ShellView();
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private void BtnCadastrar_Click(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this) as MainWindow;

        if (mainWindow == null)
            return;

        mainWindow.MainContent.Content = new CadastroUsuarioView();
    }
}