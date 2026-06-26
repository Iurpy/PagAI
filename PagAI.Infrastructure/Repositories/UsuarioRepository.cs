using Microsoft.EntityFrameworkCore;
using PagAI.Application.Interfaces;
using PagAI.Domain.Entities;
using PagAI.Infrastructure.Data;

namespace PagAI.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> BuscarPorLoginAsync(string login)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public Task CadastrarAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        return Task.CompletedTask;
    }
}