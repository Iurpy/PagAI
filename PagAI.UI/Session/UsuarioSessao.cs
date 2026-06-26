using PagAI.Domain.Entities;

namespace PagAI.UI.Session;

public static class UsuarioSessao
{
    public static Usuario? UsuarioLogado { get; set; }

    public static int UsuarioId => UsuarioLogado?.Id ?? 0;
    public static string Nome => UsuarioLogado?.Nome ?? "Usuário";
}