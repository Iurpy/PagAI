using PagAI.Domain.Enums;

namespace PagAI.Domain.Entities;

public class Parcela
{
    public int Id { get; set; }

    public int ContratoId { get; set; }

    public int Numero { get; set; }

    public decimal Valor { get; set; }

    public DateTime DataVencimento { get; set; }

    public DateTime? DataPagamento { get; set; }

    public decimal ValorPago { get; set; }

    public StatusParcela Status { get; set; } = StatusParcela.Pendente;

    public string? Observacao { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public Contrato Contrato { get; set; } = null!;

    public ICollection<Pagamento> Pagamentos { get; set; } = [];
}