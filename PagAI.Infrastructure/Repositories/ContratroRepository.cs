using Microsoft.EntityFrameworkCore;
using PagAI.Application.Interfaces;
using PagAI.Domain.Entities;
using PagAI.Domain.Enums;
using PagAI.Infrastructure.Data;

namespace PagAI.Infrastructure.Repositories;

public class ContratoRepository : IContratoRepository
{
    private readonly AppDbContext _context;

    public ContratoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task AdicionarAsync(Contrato contrato)
    {
        _context.Contratos.Add(contrato);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Contrato contrato)
    {
        _context.Contratos.Update(contrato);
        return Task.CompletedTask;
    }

    public async Task<Contrato?> BuscarPorIdAsync(int id)
    {
        return await _context.Contratos
            .Include(c => c.Cliente)
            .Include(c => c.Parcelas)
                .ThenInclude(p => p.Pagamentos)
            .FirstOrDefaultAsync(c => c.Id == id && c.Ativo);
    }

    public async Task<List<Contrato>> ListarPorUsuarioAsync(int usuarioId)
    {
        return await _context.Contratos
            .Include(c => c.Cliente)
            .Include(c => c.Parcelas)
                .ThenInclude(p => p.Pagamentos)
            .Where(c => c.Cliente.UsuarioId == usuarioId && c.Ativo)
            .OrderByDescending(c => c.DataCadastro)
            .ToListAsync();
    }

    public async Task<List<Contrato>> ListarPorClienteAsync(int clienteId)
    {
        return await _context.Contratos
            .Include(c => c.Cliente)
            .Include(c => c.Parcelas)
                .ThenInclude(p => p.Pagamentos)
            .Where(c => c.ClienteId == clienteId && c.Ativo)
            .OrderByDescending(c => c.DataCadastro)
            .ToListAsync();
    }

    public Task RemoverAsync(Contrato contrato)
    {
        contrato.Ativo = false;
        contrato.Status = StatusContrato.Cancelado;

        _context.Contratos.Update(contrato);
        return Task.CompletedTask;
    }
}