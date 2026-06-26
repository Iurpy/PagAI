using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Services;
using PagAI.UI.Services;
using PagAI.Domain.Enums;

namespace PagAI.UI.Views;

public partial class ReceberParcelaView : UserControl
{
    private readonly ContratoService _contratoService;
    private readonly int _contratoId;
    private readonly int _parcelaId;

    public ReceberParcelaView(int contratoId, int parcelaId)
    {
        InitializeComponent();

        _contratoId = contratoId;
        _parcelaId = parcelaId;
        _contratoService = App.ServiceProvider.GetRequiredService<ContratoService>();

        TxtDataPagamento.Text = DateTime.Today.ToString("dd/MM/yyyy");

        CarregarParcela();
    }

    private async void CarregarParcela()
    {
        try
        {
            var contrato = await _contratoService.BuscarPorIdAsync(_contratoId);

            if (contrato == null)
            {
                NotificationService.Error("Contrato não encontrado.");
                NavigationService.Navigate(new ContratosView());
                return;
            }

            var parcela = contrato.Parcelas.FirstOrDefault(p => p.Id == _parcelaId && p.Ativo);

            if (parcela == null)
            {
                NotificationService.Error("Parcela não encontrada.");
                NavigationService.Navigate(new ContratoDetalhesView(_contratoId));
                return;
            }

            var valorJaPago = parcela.ValorPago;
            var valorRestante = parcela.Valor - valorJaPago;

            if (valorRestante < 0)
                valorRestante = 0;

            TxtDescricaoParcela.Text = $"Parcela {parcela.Numero} - {contrato.Titulo}";
            TxtValorParcela.Text = FormatarMoeda(parcela.Valor);
            TxtValorJaPago.Text = FormatarMoeda(valorJaPago);
            TxtValorRestante.Text = FormatarMoeda(valorRestante);
            TxtValorRecebido.Text = valorRestante.ToString("N2", new CultureInfo("pt-BR"));
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private async void BtnConfirmarRecebimento_Click(object sender, RoutedEventArgs e)
{
    try
    {
        if (!decimal.TryParse(
                TxtValorRecebido.Text,
                NumberStyles.Number,
                new CultureInfo("pt-BR"),
                out var valorRecebido))
        {
            NotificationService.Error("Informe um valor válido.");
            return;
        }

        if (!DateTime.TryParseExact(
                TxtDataPagamento.Text,
                "dd/MM/yyyy",
                new CultureInfo("pt-BR"),
                DateTimeStyles.None,
                out var dataPagamento))
        {
            NotificationService.Error("Data inválida.");
            return;
        }

        var contrato = await _contratoService.BuscarPorIdAsync(_contratoId);

        var parcela = contrato!.Parcelas.First(p => p.Id == _parcelaId);

        var valorJaPago = parcela.ValorPago;

        var valorRestante = parcela.Valor - valorJaPago;

        if (valorRestante < 0)
            valorRestante = 0;

        decimal multa = 0;

        if (parcela.Status == StatusParcela.Atrasado)
        {
            multa = contrato.TipoMulta switch
            {
                TipoMulta.Percentual => valorRestante * (contrato.TaxaMulta / 100m),
                TipoMulta.ValorFixo => contrato.TaxaMulta,
                _ => 0
            };
        }

        var valorNecessario = valorRestante + multa;

        if (valorRecebido > valorNecessario)
        {
            NotificationService.Error(
                $"Valor máximo permitido: {valorNecessario:C}");
            return;
        }

        await _contratoService.ReceberParcelaAsync(
            _contratoId,
            _parcelaId,
            valorRecebido,
            dataPagamento,
            TxtObservacao.Text);

        NotificationService.Success("Pagamento registrado.");

        NavigationService.Navigate(
            new ContratoDetalhesView(_contratoId));
    }
    catch (Exception ex)
    {
        NotificationService.Error(ex.Message);
    }
}

    private void BtnVoltar_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ContratoDetalhesView(_contratoId));
    }

    private static string FormatarMoeda(decimal valor)
    {
        return valor.ToString("C", new CultureInfo("pt-BR"));
    }
}