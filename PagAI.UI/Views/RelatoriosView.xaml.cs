using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Models;
using PagAI.Application.Services;
using PagAI.Domain.Enums;
using PagAI.UI.Services;
using PagAI.UI.Session;

namespace PagAI.UI.Views;

public partial class RelatoriosView : UserControl
{
    private readonly ContratoService _contratoService;

    public RelatoriosView()
    {
        InitializeComponent();

        _contratoService = App.ServiceProvider.GetRequiredService<ContratoService>();

        CarregarRelatorios();
    }

    private async void CarregarRelatorios()
    {
        try
        {
            var relatorio = await _contratoService.ObterRelatorioAsync(UsuarioSessao.UsuarioId);

            PreencherCards(relatorio);
            PreencherContratos(relatorio.Contratos);
            PreencherRecebimentos(relatorio.Recebimentos);
            PreencherInadimplentes(relatorio.Inadimplentes);
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private void PreencherCards(RelatorioModel relatorio)
    {
        TxtTotalEmprestado.Text = FormatarMoeda(relatorio.TotalEmprestado);
        TxtTotalComJuros.Text = FormatarMoeda(relatorio.TotalComJuros);
        TxtTotalRecebido.Text = FormatarMoeda(relatorio.TotalRecebido);
        TxtTotalEmAberto.Text = FormatarMoeda(relatorio.TotalEmAberto);
        TxtLucroPrevisto.Text = FormatarMoeda(relatorio.LucroPrevisto);

        TxtTotalContratos.Text = relatorio.TotalContratos.ToString();
        TxtContratosAtivos.Text = relatorio.ContratosAtivos.ToString();
        TxtParcelasAtrasadas.Text = relatorio.ParcelasAtrasadas.ToString();
    }

    private void PreencherContratos(List<RelatorioContratoModel> contratos)
    {
        var view = contratos
            .Select(c => new RelatorioContratoViewModel
            {
                ContratoId = c.ContratoId,
                Cliente = c.Cliente,
                Titulo = c.Titulo,
                ValorTotal = FormatarMoeda(c.ValorTotal),
                Resumo = $"Recebido: {FormatarMoeda(c.ValorRecebido)} • Aberto: {FormatarMoeda(c.ValorEmAberto)}",
                Status = c.Status,
                CorStatus = c.Status == StatusContrato.Finalizado.ToString()
                    ? new SolidColorBrush(Color.FromRgb(34, 197, 94))
                    : new SolidColorBrush(Color.FromRgb(56, 189, 248)),
                DataContrato = c.DataContrato.ToLocalTime().ToString("dd/MM/yyyy")
            })
            .ToList();

        ListaContratos.ItemsSource = view;

        TxtSemContratos.Visibility = view.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PreencherRecebimentos(List<RelatorioRecebimentoModel> recebimentos)
    {
        var view = recebimentos
            .Take(8)
            .Select(r => new RelatorioRecebimentoViewModel
            {
                ContratoId = r.ContratoId,
                Cliente = r.Cliente,
                Descricao = $"{r.Contrato} • {r.Parcela}",
                Valor = FormatarMoeda(r.Valor),
                DataPagamento = r.DataPagamento.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
            })
            .ToList();

        ListaRecebimentos.ItemsSource = view;

        TxtSemRecebimentos.Visibility = view.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PreencherInadimplentes(List<RelatorioInadimplenteModel> inadimplentes)
    {
        var view = inadimplentes
            .Select(i => new RelatorioInadimplenteViewModel
            {
                ContratoId = i.ContratoId,
                Cliente = i.Cliente,
                Descricao = $"{i.Contrato} • {i.Parcela}",
                Valor = FormatarMoeda(i.ValorRestante),
                Atraso = i.DiasAtraso == 1
                    ? "Atrasada há 1 dia"
                    : $"Atrasada há {i.DiasAtraso} dias"
            })
            .ToList();

        ListaInadimplentes.ItemsSource = view;

        TxtSemInadimplentes.Visibility = view.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void AbrirContrato_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement elemento)
            return;

        var contratoId = elemento.DataContext switch
        {
            RelatorioContratoViewModel contrato => contrato.ContratoId,
            RelatorioRecebimentoViewModel recebimento => recebimento.ContratoId,
            RelatorioInadimplenteViewModel inadimplente => inadimplente.ContratoId,
            _ => 0
        };

        if (contratoId <= 0)
            return;

        NavigationService.Navigate(new ContratoDetalhesView(contratoId));
    }

    private static string FormatarMoeda(decimal valor)
    {
        return valor.ToString("C", new CultureInfo("pt-BR"));
    }

    private class RelatorioContratoViewModel
    {
        public int ContratoId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;

        public string ValorTotal { get; set; } = string.Empty;

        public string Resumo { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public Brush CorStatus { get; set; } = Brushes.White;

        public string DataContrato { get; set; } = string.Empty;
    }

    private class RelatorioRecebimentoViewModel
    {
        public int ContratoId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string Valor { get; set; } = string.Empty;

        public string DataPagamento { get; set; } = string.Empty;
    }

    private class RelatorioInadimplenteViewModel
    {
        public int ContratoId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string Valor { get; set; } = string.Empty;

        public string Atraso { get; set; } = string.Empty;
    }
}