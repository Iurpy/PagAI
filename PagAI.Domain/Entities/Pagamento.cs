namespace PagAI.Domain.Entities;

public class Pagamento
{
    public int Id { get; set; }

    public int ParcelaId { get; set; }

    public decimal Valor { get; set; }

    public DateTime DataPagamento { get; set; }

    public string? Observacao { get; set; }

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public Parcela Parcela { get; set; } = null!;
}