namespace PagAI.Application.Models;

public class DashboardModel
{
    public decimal CapitalEmprestado { get; set; }
    public decimal TotalRecebido { get; set; }
    public decimal TotalEmAberto { get; set; }

    public int ClientesComContrato { get; set; }
    public int ContratosAtivos { get; set; }
    public int ContratosFinalizados { get; set; }
    public int ParcelasAtrasadas { get; set; }

    public int ParcelasVencendoHoje { get; set; }

    public List<DashboardVencimentoModel> ProximosVencimentos { get; set; } = [];
    public List<DashboardRecebimentoModel> UltimosRecebimentos { get; set; } = [];
    public List<DashboardContratoModel> ContratosRecentes { get; set; } = [];
}

public class DashboardVencimentoModel
{
    public int ContratoId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Contrato { get; set; } = string.Empty;

    public string Parcela { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime Vencimento { get; set; }

    public bool EstaAtrasada { get; set; }

    public bool VenceHoje { get; set; }
}

public class DashboardRecebimentoModel
{
    public int ContratoId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Contrato { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime DataPagamento { get; set; }
}

public class DashboardContratoModel
{
    public int ContratoId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public decimal ValorTotal { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; }
}