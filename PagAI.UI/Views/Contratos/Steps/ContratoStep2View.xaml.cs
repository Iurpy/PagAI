using System.Globalization;
using System.Windows;
using PagAI.Domain.Enums;
using PagAI.UI.Components;

namespace PagAI.UI.Views.Contratos.Steps;

public partial class ContratoStep2View : WizardStep
{
    public ContratoStep2View()
    {
        InitializeComponent();

        CmbTipoPagamento.ItemsSource = Enum.GetValues(typeof(TipoPagamento));
        CmbPeriodicidade.ItemsSource = Enum.GetValues(typeof(TipoPeriodicidade));
    }

    public override void Carregar()
    {
        CmbTipoPagamento.SelectedItem = Model.TipoPagamento;
        CmbPeriodicidade.SelectedItem = Model.Periodicidade;

        TxtQuantidadeParcelas.Text = Model.QuantidadeParcelas.ToString();

        var vencimento = Model.PrimeiroVencimento == default
            ? DateTime.Today
            : Model.PrimeiroVencimento;

        TxtPrimeiroVencimento.Text = vencimento.ToString("dd/MM/yyyy");

        AtualizarCamposPorTipoPagamento();
    }

    public override void Salvar()
    {
        if (CmbTipoPagamento.SelectedItem is TipoPagamento tipoPagamento)
            Model.TipoPagamento = tipoPagamento;

        if (DateTime.TryParseExact(
                TxtPrimeiroVencimento.Text.Trim(),
                "dd/MM/yyyy",
                new CultureInfo("pt-BR"),
                DateTimeStyles.None,
                out var vencimento))
        {
            Model.PrimeiroVencimento = vencimento;
        }

        if (Model.TipoPagamento == TipoPagamento.AVista)
        {
            Model.QuantidadeParcelas = 1;
            Model.Periodicidade = TipoPeriodicidade.Unica;
            return;
        }

        if (CmbPeriodicidade.SelectedItem is TipoPeriodicidade periodicidade)
            Model.Periodicidade = periodicidade;

        if (int.TryParse(TxtQuantidadeParcelas.Text, out var quantidade))
            Model.QuantidadeParcelas = quantidade;
    }

    public override bool Validar()
    {
        Salvar();

        if (!DateTime.TryParseExact(
                TxtPrimeiroVencimento.Text.Trim(),
                "dd/MM/yyyy",
                new CultureInfo("pt-BR"),
                DateTimeStyles.None,
                out _))
        {
            throw new Exception("Informe uma data válida no formato dd/mm/aaaa.");
        }

        if (Model.TipoPagamento == TipoPagamento.Parcelado && Model.QuantidadeParcelas < 2)
            throw new Exception("Para pagamento parcelado, informe pelo menos 2 parcelas.");

        if (Model.PrimeiroVencimento == default)
            throw new Exception("Informe o vencimento.");

        return true;
    }

    private void CmbTipoPagamento_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        AtualizarCamposPorTipoPagamento();
    }

    private void AtualizarCamposPorTipoPagamento()
    {
        if (CmbTipoPagamento.SelectedItem is not TipoPagamento tipoPagamento)
            return;

        if (tipoPagamento == TipoPagamento.AVista)
        {
            PainelParcelado.Visibility = Visibility.Collapsed;
            LblVencimentoAVista.Visibility = Visibility.Visible;

            TxtQuantidadeParcelas.Text = "1";
            CmbPeriodicidade.SelectedItem = TipoPeriodicidade.Unica;

            return;
        }

        PainelParcelado.Visibility = Visibility.Visible;
        LblVencimentoAVista.Visibility = Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(TxtQuantidadeParcelas.Text) || TxtQuantidadeParcelas.Text == "1")
            TxtQuantidadeParcelas.Text = "2";

        if (CmbPeriodicidade.SelectedItem is null || (TipoPeriodicidade)CmbPeriodicidade.SelectedItem == TipoPeriodicidade.Unica)
            CmbPeriodicidade.SelectedItem = TipoPeriodicidade.Mensal;
    }
}