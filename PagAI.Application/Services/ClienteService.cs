using PagAI.Application.Interfaces;
using PagAI.Domain.Entities;

namespace PagAI.Application.Services;

public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ClienteService(
        IClienteRepository clienteRepository,
        IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CadastrarAsync(Cliente cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente.Nome))
            throw new Exception("Informe o nome do cliente.");

        cliente.Nome = cliente.Nome.Trim();

        await _clienteRepository.AdicionarAsync(cliente);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<Cliente>> ListarPorUsuarioAsync(int usuarioId)
    {
        return await _clienteRepository.ListarPorUsuarioAsync(usuarioId);
    }
}