using PagAI.Domain.Entities;

namespace PagAI.Application.Interfaces;

public interface IClienteRepository
{
    Task AdicionarAsync(Cliente cliente);
    Task<List<Cliente>> ListarPorUsuarioAsync(int usuarioId);
}