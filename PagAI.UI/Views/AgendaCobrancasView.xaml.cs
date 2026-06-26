using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Models;
using PagAI.Application.Services;
using PagAI.UI.Services;
using PagAI.UI.Session;

namespace PagAI.UI.Views;

public partial class AgendaCobrancasView : UserControl
{
    private readonly ContratoService _contratoService;

    public AgendaCobrancasView()
    {
        InitializeComponent();

        _contratoService = App.ServiceProvider.GetRequiredService<ContratoService>();

        CarregarAgenda();
    }

    private async void CarregarAgenda()
    {
        try
        {
            var agenda = await _contratoService.ObterAgendaCobrancasAsync(UsuarioSessao.UsuarioId);

            PreencherCards(agenda);
            PreencherAtrasadas(agenda.Atrasadas);
            PreencherHoje(agenda.VencendoHoje);
            PreencherProximos(agenda.ProximosDias);
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private void PreencherCards(AgendaCobrancasModel agenda)
    {
        TxtTotalAtrasadas.Text = agenda.TotalAtrasadas.ToString();
        TxtTotalHoje.Text = agenda.TotalVencendoHoje.ToString();
        TxtTotalProximos.Text = agenda.TotalProximosDias.ToString();

        TxtValorAtrasado.Text = FormatarMoeda(agenda.ValorAtrasado);
        TxtValorHoje.Text = FormatarMoeda(agenda.ValorVencendoHoje);
        TxtValorProximos.Text = FormatarMoeda(agenda.ValorProximosDias);
    }

    private void PreencherAtrasadas(List<AgendaCobrancaItemModel> itens)
    {
        var view = itens
            .Select(x => new AgendaCobrancaViewModel
            {
                ContratoId = x.ContratoId,
                ParcelaId = x.ParcelaId,
                Cliente = x.Cliente,
                Descricao = $"{x.Contrato} • {x.Parcela}",
                Valor = FormatarMoeda(x.ValorRestante),
                Situacao = x.Dias == 1
                    ? "Atrasada há 1 dia"
                    : $"Atrasada há {x.Dias} dias"
            })
            .ToList();

        ListaAtrasadas.ItemsSource = view;

        TxtSemAtrasadas.Visibility = view.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PreencherHoje(List<AgendaCobrancaItemModel> itens)
    {
        var view = itens
            .Select(x => new AgendaCobrancaViewModel
            {
                ContratoId = x.ContratoId,
                ParcelaId = x.ParcelaId,
                Cliente = x.Cliente,
                Descricao = $"{x.Contrato} • {x.Parcela}",
                Valor = FormatarMoeda(x.ValorRestante),
                Situacao = "Vence hoje"
            })
            .ToList();

        ListaHoje.ItemsSource = view;

        TxtSemHoje.Visibility = view.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PreencherProximos(List<AgendaCobrancaItemModel> itens)
    {
        var view = itens
            .Select(x => new AgendaCobrancaViewModel
            {
                ContratoId = x.ContratoId,
                ParcelaId = x.ParcelaId,
                Cliente = x.Cliente,
                Descricao = $"{x.Contrato} • {x.Parcela}",
                Valor = FormatarMoeda(x.ValorRestante),
                Situacao = x.Dias == 1
                    ? "Vence amanhã"
                    : $"Vence em {x.Dias} dias"
            })
            .ToList();

        ListaProximos.ItemsSource = view;

        TxtSemProximos.Visibility = view.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void AbrirContrato_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement elemento)
            return;

        if (elemento.DataContext is not AgendaCobrancaViewModel item)
            return;

        NavigationService.Navigate(new ContratoDetalhesView(item.ContratoId));
    }

    private static string FormatarMoeda(decimal valor)
    {
        return valor.ToString("C", new CultureInfo("pt-BR"));
    }

    private class AgendaCobrancaViewModel
    {
        public int ContratoId { get; set; }

        public int ParcelaId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string Valor { get; set; } = string.Empty;

        public string Situacao { get; set; } = string.Empty;
    }
}