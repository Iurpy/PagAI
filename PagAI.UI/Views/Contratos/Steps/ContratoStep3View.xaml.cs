using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using PagAI.Domain.Enums;
using PagAI.UI.Components;

namespace PagAI.UI.Views.Contratos.Steps;

public partial class ContratoStep3View : WizardStep
{
    public ContratoStep3View()
    {
        InitializeComponent();

        CmbTipoCalculoJuros.ItemsSource = Enum.GetValues(typeof(TipoCalculoJuros));
        CmbTipoCobrancaJuros.ItemsSource = Enum.GetValues(typeof(TipoCobrancaJuros));
        CmbTipoMulta.ItemsSource = Enum.GetValues(typeof(TipoMulta));
    }

    public override void Carregar()
    {
        CmbTipoCalculoJuros.SelectedItem = Model.TipoCalculoJuros;
        CmbTipoCobrancaJuros.SelectedItem = Model.TipoCobrancaJuros;
        CmbTipoMulta.SelectedItem = Model.TipoMulta;

        TxtTaxaJuros.Text = Model.TaxaJuros > 0
            ? Model.TaxaJuros.ToString("N2", new CultureInfo("pt-BR"))
            : "0,00";

        TxtTaxaMulta.Text = Model.TaxaMulta > 0
            ? Model.TaxaMulta.ToString("N2", new CultureInfo("pt-BR"))
            : "0,00";

        AtualizarCamposPorTipoMulta();
    }

    public override void Salvar()
    {
        if (CmbTipoCalculoJuros.SelectedItem is TipoCalculoJuros tipoCalculoJuros)
            Model.TipoCalculoJuros = tipoCalculoJuros;

        if (CmbTipoCobrancaJuros.SelectedItem is TipoCobrancaJuros tipoCobrancaJuros)
            Model.TipoCobrancaJuros = tipoCobrancaJuros;

        if (CmbTipoMulta.SelectedItem is TipoMulta tipoMulta)
            Model.TipoMulta = tipoMulta;

        Model.TaxaJuros = ConverterDecimal(TxtTaxaJuros.Text);
        Model.TaxaMulta = ConverterDecimal(TxtTaxaMulta.Text);

        if (Model.TipoMulta == TipoMulta.SemMulta)
            Model.TaxaMulta = 0;
    }

    public override bool Validar()
    {
        Salvar();

        if (Model.TaxaJuros < 0)
            throw new Exception("A taxa de juros não pode ser negativa.");

        if (Model.TaxaMulta < 0)
            throw new Exception("A multa não pode ser negativa.");

        return true;
    }

    private void CmbTipoMulta_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        AtualizarCamposPorTipoMulta();
    }

    private void AtualizarCamposPorTipoMulta()
    {
        if (CmbTipoMulta.SelectedItem is not TipoMulta tipoMulta)
            return;

        if (tipoMulta == TipoMulta.SemMulta)
        {
            PainelTaxaMulta.Visibility = Visibility.Collapsed;
            TxtTaxaMulta.Text = "0,00";
            return;
        }

        PainelTaxaMulta.Visibility = Visibility.Visible;

        LblPrefixoMulta.Text = tipoMulta == TipoMulta.Percentual
            ? "%"
            : "R$";
    }

    private static decimal ConverterDecimal(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return 0;

        texto = texto.Trim();

        if (decimal.TryParse(texto, NumberStyles.Number, new CultureInfo("pt-BR"), out var valorPtBr))
            return valorPtBr;

        if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out var valorInvariant))
            return valorInvariant;

        return 0;
    }
}