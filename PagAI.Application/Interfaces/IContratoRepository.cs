using PagAI.Domain.Entities;

namespace PagAI.Application.Interfaces;

public interface IContratoRepository
{
    Task AdicionarAsync(Contrato contrato);

    Task AtualizarAsync(Contrato contrato);

    Task<Contrato?> BuscarPorIdAsync(int id);

    Task<List<Contrato>> ListarPorUsuarioAsync(int usuarioId);

    Task<List<Contrato>> ListarPorClienteAsync(int clienteId);

    Task RemoverAsync(Contrato contrato);
}