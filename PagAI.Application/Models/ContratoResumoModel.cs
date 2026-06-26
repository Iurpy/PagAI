namespace PagAI.Application.Models;

public class ContratoResumoModel
{
    public decimal ValorEmprestado { get; set; }

    public decimal ValorTotalComJuros { get; set; }

    public decimal ValorParcela { get; set; }

    public List<ParcelaResumoModel> Parcelas { get; set; } = [];
}