using Microsoft.EntityFrameworkCore;
using PagAI.Application.Interfaces;
using PagAI.Domain.Entities;
using PagAI.Infrastructure.Data;

namespace PagAI.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly AppDbContext _context;

    public ClienteRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task AdicionarAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        return Task.CompletedTask;
    }

    public async Task<List<Cliente>> ListarPorUsuarioAsync(int usuarioId)
    {
        return await _context.Clientes
            .Where(c => c.UsuarioId == usuarioId && c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }
}