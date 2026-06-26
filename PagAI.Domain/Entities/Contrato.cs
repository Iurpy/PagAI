using PagAI.Domain.Enums;

namespace PagAI.Domain.Entities;

public class Contrato
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public decimal ValorEmprestado { get; set; }

    public decimal ValorTotalComJuros { get; set; }

    public DateTime DataContrato { get; set; } = DateTime.UtcNow;

    public DateTime PrimeiroVencimento { get; set; }

    public TipoPagamento TipoPagamento { get; set; } = TipoPagamento.Parcelado;

    public int QuantidadeParcelas { get; set; } = 1;

    public TipoPeriodicidade Periodicidade { get; set; } = TipoPeriodicidade.Mensal;

    public TipoCalculoJuros TipoCalculoJuros { get; set; } = TipoCalculoJuros.Simples;

    public TipoCobrancaJuros TipoCobrancaJuros { get; set; } = TipoCobrancaJuros.SobreTotal;

    public decimal TaxaJuros { get; set; }

    public TipoMulta TipoMulta { get; set; } = TipoMulta.Percentual;

    public decimal TaxaMulta { get; set; }

    public string? Observacao { get; set; }

    public StatusContrato Status { get; set; } = StatusContrato.Ativo;

    public bool Ativo { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public Cliente Cliente { get; set; } = null!;

    public ICollection<Parcela> Parcelas { get; set; } = [];
}