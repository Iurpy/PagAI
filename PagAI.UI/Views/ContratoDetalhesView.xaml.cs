using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Services;
using PagAI.Domain.Entities;
using PagAI.Domain.Enums;
using PagAI.UI.Services;

namespace PagAI.UI.Views;

public partial class ContratoDetalhesView : UserControl
{
    private readonly ContratoService _contratoService;
    private readonly int _contratoId;

    public ContratoDetalhesView(int contratoId)
    {
        InitializeComponent();

        _contratoId = contratoId;
        _contratoService = App.ServiceProvider.GetRequiredService<ContratoService>();

        CarregarContrato();
    }

    private async void CarregarContrato()
{
    try
    {
        await _contratoService.AtualizarParcelasAtrasadasAsync(_contratoId);

        var contrato = await _contratoService.BuscarPorIdAsync(_contratoId);

        if (contrato == null)
        {
            NotificationService.Error("Contrato não encontrado.");
            NavigationService.Navigate(new ContratosView());
            return;
        }

        PreencherTela(contrato);
    }
    catch (Exception ex)
    {
        NotificationService.Error(ex.Message);
    }
}

    private void PreencherTela(Contrato contrato)
    {
        TxtTitulo.Text = contrato.Titulo;
        TxtCliente.Text = contrato.Cliente.Nome;
        TxtStatus.Text = contrato.Status.ToString();

        TxtValorEmprestado.Text = FormatarMoeda(contrato.ValorEmprestado);
        TxtValorTotal.Text = FormatarMoeda(contrato.ValorTotalComJuros);
        TxtQuantidadeParcelas.Text = contrato.QuantidadeParcelas.ToString();

        TxtDataContrato.Text = contrato.DataContrato.ToLocalTime().ToString("dd/MM/yyyy");
        TxtPrimeiroVencimento.Text = contrato.PrimeiroVencimento.ToLocalTime().ToString("dd/MM/yyyy");

        TxtTipoPagamento.Text = contrato.TipoPagamento.ToString();
        TxtPeriodicidade.Text = contrato.Periodicidade.ToString();

        PreencherResumoParcelas(contrato.Parcelas);
        PreencherListaParcelas(contrato);
    }

private void PreencherResumoParcelas(IEnumerable<Parcela> parcelas)
{
    var parcelasAtivas = parcelas
        .Where(p => p.Ativo)
        .ToList();

    TxtParcelasPagas.Text = parcelasAtivas.Count(p => p.Status == StatusParcela.Pago).ToString();
    TxtParcelasPendentes.Text = parcelasAtivas.Count(p => p.Status == StatusParcela.Pendente).ToString();
    TxtParcelasAtrasadas.Text = parcelasAtivas.Count(p => p.Status == StatusParcela.Atrasado).ToString();

    var totalRecebido = parcelasAtivas.Sum(p => p.ValorPago);

    var totalEmAberto = parcelasAtivas.Sum(p =>
    {
        var restante = p.Valor - (p.ValorPago);
        return restante < 0 ? 0 : restante;
    });

    TxtValorRecebido.Text = FormatarMoeda(totalRecebido);
    TxtValorEmAberto.Text = FormatarMoeda(totalEmAberto);
}

private void PreencherListaParcelas(Contrato contrato)
{
    var parcelasView = contrato.Parcelas
        .Where(p => p.Ativo)
        .OrderBy(p => p.Numero)
        .Select(p =>
        {
            var valorPago = p.ValorPago;
            var valorRestante = p.Valor - valorPago;

            if (valorRestante < 0)
                valorRestante = 0;

            var diasAtraso = p.Status == StatusParcela.Atrasado
                ? Math.Max(0, (DateTime.Today - p.DataVencimento.ToLocalTime().Date).Days)
                : 0;

            var valorMulta = CalcularMulta(contrato, valorRestante, diasAtraso);
            var valorComMulta = valorRestante + valorMulta;

            return new ParcelaViewModel
            {
                ParcelaId = p.Id,
                Numero = $"Parcela {p.Numero}",
                Vencimento = $"Vencimento: {p.DataVencimento.ToLocalTime():dd/MM/yyyy}",
                Valor = FormatarMoeda(p.Valor),
                ValorPago = $"Pago: {FormatarMoeda(valorPago)}",
                ValorRestante = $"Restante: {FormatarMoeda(valorRestante)}",
                Multa = valorMulta > 0 ? $"Multa: {FormatarMoeda(valorMulta)}" : "",
                ValorComMulta = valorMulta > 0 ? $"Total a receber: {FormatarMoeda(valorComMulta)}" : "",
                Status = p.Status.ToString(),
                Atraso = diasAtraso > 0 ? $"Atraso: {diasAtraso} dia(s)" : "",
                PodeReceber = p.Status != StatusParcela.Pago,
                HistoricoPagamentos = p.Pagamentos
                                .OrderBy(pg => pg.DataPagamento)
                                .Select(pg => new PagamentoViewModel
                                {
                                    Data = pg.DataPagamento.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                                    Valor = FormatarMoeda(pg.Valor),
                                    Observacao = string.IsNullOrWhiteSpace(pg.Observacao)
                                        ? "Pagamento registrado"
                                        : pg.Observacao
                                })
                                .ToList()
            };
        })
        .ToList();

    ListaParcelas.ItemsSource = parcelasView;
}

    private void BtnReceberParcela_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button botao)
        {
            NotificationService.Error("Erro ao abrir recebimento.");
            return;
        }

        if (botao.DataContext is not ParcelaViewModel parcela)
        {
            NotificationService.Error("Erro ao identificar a parcela.");
            return;
        }

        NavigationService.Navigate(
            new ReceberParcelaView(
                _contratoId,
                parcela.ParcelaId));
    }

    private async void BtnQuitarContrato_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await _contratoService.QuitarContratoAsync(
                _contratoId,
                DateTime.Today);

            NotificationService.Success("Contrato quitado com sucesso.");

            NavigationService.Navigate(new ContratoDetalhesView(_contratoId));
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private static string FormatarMoeda(decimal valor)
    {
        return valor.ToString("C", new CultureInfo("pt-BR"));
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new ContratosView());
    }

private static decimal CalcularMulta(Contrato contrato, decimal valorRestante, int diasAtraso)
{
    if (diasAtraso <= 0 || valorRestante <= 0)
        return 0;

    return contrato.TipoMulta switch
    {
        TipoMulta.Percentual => valorRestante * (contrato.TaxaMulta / 100m),
        TipoMulta.ValorFixo => contrato.TaxaMulta,
        _ => 0
    };
}

private async void BtnGerarPdf_Click(object sender, RoutedEventArgs e)
{
    try
    {
        var contrato = await _contratoService.BuscarPorIdAsync(_contratoId);

        if (contrato == null)
        {
            NotificationService.Error("Contrato não encontrado.");
            return;
        }

        var caminho = ContratoPdfService.GerarComprovanteContrato(contrato);

        NotificationService.Success($"PDF gerado em: {caminho}");
    }
    catch (Exception ex)
    {
        NotificationService.Error(ex.Message);
    }
}
private class ParcelaViewModel
{
    public int ParcelaId { get; set; }

    public string Numero { get; set; } = string.Empty;

    public string Vencimento { get; set; } = string.Empty;

    public string Valor { get; set; } = string.Empty;

    public string ValorPago { get; set; } = string.Empty;

    public string ValorRestante { get; set; } = string.Empty;

    public string Multa { get; set; } = string.Empty;

    public string ValorComMulta { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Atraso { get; set; } = string.Empty;

    public bool PodeReceber { get; set; }

    public List<PagamentoViewModel> HistoricoPagamentos { get; set; } = [];
}

private class PagamentoViewModel
{
    public string Data { get; set; } = string.Empty;

    public string Valor { get; set; } = string.Empty;

    public string Observacao { get; set; } = string.Empty;
}
}