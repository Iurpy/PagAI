using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Calculators;
using PagAI.UI.Components;

namespace PagAI.UI.Views.Contratos.Steps;

public partial class ContratoStep4View : WizardStep
{
    private readonly ContratoCalculator _contratoCalculator;

    public ContratoStep4View()
    {
        InitializeComponent();

        _contratoCalculator = App.ServiceProvider.GetRequiredService<ContratoCalculator>();
    }

    public override void Carregar()
    {
        var cultura = new CultureInfo("pt-BR");

        var resumo = _contratoCalculator.GerarResumo(Model);

        TxtValorEmprestado.Text = resumo.ValorEmprestado.ToString("C2", cultura);
        TxtValorTotal.Text = resumo.ValorTotalComJuros.ToString("C2", cultura);
        TxtParcelas.Text = Model.TipoPagamento.ToString() == "AVista"
            ? "À vista"
            : $"{Model.QuantidadeParcelas}x";

        TxtTotalParcelas.Text = $"{resumo.Parcelas.Count} parcela(s)";

        ListaParcelas.ItemsSource = resumo.Parcelas;
    }
}