using PagAI.Application.Models;
using PagAI.Domain.Entities;
using PagAI.Domain.Enums;

namespace PagAI.Application.Calculators;

public class ContratoCalculator
{
    public ContratoResumoModel GerarResumo(ContratoWizardModel model)
    {
        var valorTotal = CalcularValorTotalComJuros(
            model.ValorEmprestado,
            model.TaxaJuros,
            model.TipoCobrancaJuros,
            model.QuantidadeParcelas
        );

        var parcelasResumo = GerarParcelasResumo(
            valorTotal,
            model.QuantidadeParcelas,
            model.PrimeiroVencimento,
            model.Periodicidade
        );

        return new ContratoResumoModel
        {
            ValorEmprestado = model.ValorEmprestado,
            ValorTotalComJuros = valorTotal,
            ValorParcela = parcelasResumo.Count > 0 ? parcelasResumo[0].Valor : 0,
            Parcelas = parcelasResumo
        };
    }

    public decimal CalcularValorTotalComJuros(Contrato contrato)
    {
        return CalcularValorTotalComJuros(
            contrato.ValorEmprestado,
            contrato.TaxaJuros,
            contrato.TipoCobrancaJuros,
            contrato.QuantidadeParcelas
        );
    }

    public List<Parcela> GerarParcelas(Contrato contrato)
    {
        var parcelasResumo = GerarParcelasResumo(
            contrato.ValorTotalComJuros,
            contrato.QuantidadeParcelas,
            contrato.PrimeiroVencimento,
            contrato.Periodicidade
        );

        return parcelasResumo.Select(p => new Parcela
        {
            Numero = p.Numero,
            Valor = p.Valor,
            DataVencimento = p.DataVencimento,
            Status = StatusParcela.Pendente,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        }).ToList();
    }

    private static decimal CalcularValorTotalComJuros(
        decimal valorEmprestado,
        decimal taxaJuros,
        TipoCobrancaJuros tipoCobrancaJuros,
        int quantidadeParcelas)
    {
        if (taxaJuros <= 0)
            return valorEmprestado;

        if (tipoCobrancaJuros == TipoCobrancaJuros.SobreTotal)
        {
            return valorEmprestado + CalcularJuros(valorEmprestado, taxaJuros);
        }

        decimal valorBaseParcela = valorEmprestado / quantidadeParcelas;
        decimal valorParcelaComJuros = valorBaseParcela + CalcularJuros(valorBaseParcela, taxaJuros);

        return valorParcelaComJuros * quantidadeParcelas;
    }

    private static List<ParcelaResumoModel> GerarParcelasResumo(
        decimal valorTotal,
        int quantidadeParcelas,
        DateTime primeiroVencimento,
        TipoPeriodicidade periodicidade)
    {
        var parcelas = new List<ParcelaResumoModel>();

        decimal valorParcela = valorTotal / quantidadeParcelas;
        decimal somaParcelas = 0;

        for (int i = 1; i <= quantidadeParcelas; i++)
        {
            decimal valor = Math.Round(valorParcela, 2);

            if (i == quantidadeParcelas)
                valor = Math.Round(valorTotal - somaParcelas, 2);

            somaParcelas += valor;

            parcelas.Add(new ParcelaResumoModel
            {
                Numero = i,
                Valor = valor,
                DataVencimento = CalcularVencimento(primeiroVencimento, periodicidade, i)
            });
        }

        return parcelas;
    }

    private static decimal CalcularJuros(decimal valor, decimal taxaJuros)
    {
        return valor * (taxaJuros / 100);
    }

    private static DateTime CalcularVencimento(
        DateTime primeiroVencimento,
        TipoPeriodicidade periodicidade,
        int numeroParcela)
    {
        int indice = numeroParcela - 1;

        return periodicidade switch
        {
            TipoPeriodicidade.Unica => primeiroVencimento,
            TipoPeriodicidade.Semanal => primeiroVencimento.AddDays(7 * indice),
            TipoPeriodicidade.Quinzenal => primeiroVencimento.AddDays(15 * indice),
            TipoPeriodicidade.Mensal => primeiroVencimento.AddMonths(indice),
            TipoPeriodicidade.Bimestral => primeiroVencimento.AddMonths(2 * indice),
            TipoPeriodicidade.Trimestral => primeiroVencimento.AddMonths(3 * indice),
            TipoPeriodicidade.Semestral => primeiroVencimento.AddMonths(6 * indice),
            TipoPeriodicidade.Anual => primeiroVencimento.AddYears(indice),
            _ => primeiroVencimento
        };
    }
}