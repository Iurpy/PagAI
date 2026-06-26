namespace PagAI.Application.Models;

public class AgendaCobrancasModel
{
    public int TotalAtrasadas { get; set; }

    public int TotalVencendoHoje { get; set; }

    public int TotalProximosDias { get; set; }

    public decimal ValorAtrasado { get; set; }

    public decimal ValorVencendoHoje { get; set; }

    public decimal ValorProximosDias { get; set; }

    public List<AgendaCobrancaItemModel> Atrasadas { get; set; } = [];

    public List<AgendaCobrancaItemModel> VencendoHoje { get; set; } = [];

    public List<AgendaCobrancaItemModel> ProximosDias { get; set; } = [];
}

public class AgendaCobrancaItemModel
{
    public int ContratoId { get; set; }

    public int ParcelaId { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string Contrato { get; set; } = string.Empty;

    public string Parcela { get; set; } = string.Empty;

    public decimal ValorRestante { get; set; }

    public DateTime DataVencimento { get; set; }

    public int Dias { get; set; }

    public string Status { get; set; } = string.Empty;
}