using PagAI.Domain.Entities;

namespace PagAI.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorLoginAsync(string login);
    Task CadastrarAsync(Usuario usuario);
}