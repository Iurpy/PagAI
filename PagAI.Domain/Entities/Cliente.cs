namespace PagAI.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Telefone { get; set; }

    public string? CPF { get; set; }

    public string? EnderecoRua { get; set; }

    public string? Apelido { get; set; }

    public string? Observacao { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;

    public ICollection<Contrato> Contratos { get; set; } = [];
}