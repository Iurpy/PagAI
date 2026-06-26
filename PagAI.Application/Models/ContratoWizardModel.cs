using PagAI.Domain.Enums;

namespace PagAI.Application.Models;

public class ContratoWizardModel
{
    // Passo 1
    public int ClienteId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public decimal ValorEmprestado { get; set; }

    // Passo 2
    public TipoPagamento TipoPagamento { get; set; } = TipoPagamento.Parcelado;

    public int QuantidadeParcelas { get; set; } = 1;

    public TipoPeriodicidade Periodicidade { get; set; } = TipoPeriodicidade.Mensal;

    public DateTime PrimeiroVencimento { get; set; } = DateTime.Today;

    // Passo 3
    public TipoCalculoJuros TipoCalculoJuros { get; set; } = TipoCalculoJuros.Simples;

    public TipoCobrancaJuros TipoCobrancaJuros { get; set; } = TipoCobrancaJuros.SobreTotal;

    public decimal TaxaJuros { get; set; }

    public TipoMulta TipoMulta { get; set; } = TipoMulta.SemMulta;

    public decimal TaxaMulta { get; set; }

    public string? Observacao { get; set; }
}