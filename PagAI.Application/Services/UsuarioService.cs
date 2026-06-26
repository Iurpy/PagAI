using PagAI.Application.Interfaces;
using PagAI.Domain.Entities;

namespace PagAI.Application.Services;

public class UsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioService(
        IUsuarioRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task CadastrarAsync(string nome, string login, string senha)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Exception("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(login))
            throw new Exception("Login é obrigatório.");

        if (string.IsNullOrWhiteSpace(senha))
            throw new Exception("Senha é obrigatória.");

        var usuarioExiste = await _repository.BuscarPorLoginAsync(login);

        if (usuarioExiste != null)
            throw new Exception("Login já existe.");

        var usuario = new Usuario
        {
            Nome = nome.Trim(),
            Login = login.Trim(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha)
        };

        await _repository.CadastrarAsync(usuario);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Usuario> LoginAsync(string login, string senha)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new Exception("Login ou senha inválidos.");

        if (string.IsNullOrWhiteSpace(senha))
            throw new Exception("Login ou senha inválidos.");

        var usuario = await _repository.BuscarPorLoginAsync(login.Trim());

        if (usuario == null)
            throw new Exception("Login ou senha inválidos.");

        bool senhaCorreta = BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash);

        if (!senhaCorreta)
            throw new Exception("Login ou senha inválidos.");

        return usuario;
    }
}