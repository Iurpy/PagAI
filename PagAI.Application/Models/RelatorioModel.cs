namespace PagAI.Application.Models;

public class RelatorioModel
{
    public decimal TotalEmprestado { get; set; }

    public decimal TotalComJuros { get; set; }

    public decimal TotalRecebido { get; set; }

    public decimal TotalEmAberto { get; set; }

    public decimal LucroPrevisto { get; set; }

    public int TotalContratos { get; set; }

    public int ContratosAtivos { get; set; }

    public int ContratosFinalizados { get; set; }

    public int ParcelasAtrasadas { get; set; }

    public List<RelatorioContratoModel> Contratos { get; set; } = [];

    public List<RelatorioRecebimentoModel> Recebimentos { get; set; } = [];

    public List<RelatorioInadimplenteModel> Inadimplentes { get; set; } = [];
}

public class RelatorioContratoModel
{
    public int ContratoId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public decimal ValorEmprestado { get; set; }

    public decimal ValorTotal { get; set; }

    public decimal ValorRecebido { get; set; }

    public decimal ValorEmAberto { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime DataContrato { get; set; }
}

public class RelatorioRecebimentoModel
{
    public int ContratoId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Contrato { get; set; } = string.Empty;

    public string Parcela { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime DataPagamento { get; set; }

    public string? Observacao { get; set; }
}

public class RelatorioInadimplenteModel
{
    public int ContratoId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Contrato { get; set; } = string.Empty;

    public string Parcela { get; set; } = string.Empty;

    public decimal ValorRestante { get; set; }

    public DateTime DataVencimento { get; set; }

    public int DiasAtraso { get; set; }
}