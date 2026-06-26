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

public partial class DashboardView : UserControl
{
    private readonly ContratoService _contratoService;

    public DashboardView()
    {
        InitializeComponent();

        _contratoService = App.ServiceProvider.GetRequiredService<ContratoService>();

        TxtNomeUsuario.Text = UsuarioSessao.Nome;

        CarregarDashboard();
    }

    private async void CarregarDashboard()
    {
        try
        {
            var dados = await _contratoService.ObterDashboardAsync(UsuarioSessao.UsuarioId);

            PreencherCards(dados);
            PreencherVencimentos(dados.ProximosVencimentos);
            PreencherRecebimentos(dados.UltimosRecebimentos);
            PreencherContratosRecentes(dados.ContratosRecentes);
        }
        catch (Exception ex)
        {
            NotificationService.Error(ex.Message);
        }
    }

    private void PreencherCards(DashboardModel dados)
    {
        TxtCapitalEmprestado.Text = FormatarMoeda(dados.CapitalEmprestado);
        TxtTotalRecebido.Text = FormatarMoeda(dados.TotalRecebido);
        TxtTotalEmAberto.Text = FormatarMoeda(dados.TotalEmAberto);

        TxtClientesComContrato.Text = dados.ClientesComContrato.ToString();
        TxtContratosAtivos.Text = dados.ContratosAtivos.ToString();
        TxtContratosFinalizados.Text = dados.ContratosFinalizados.ToString();
        TxtParcelasAtrasadas.Text = dados.ParcelasAtrasadas.ToString();
    }

    private void PreencherVencimentos(List<DashboardVencimentoModel> vencimentos)
    {
        var itens = vencimentos
            .Select(v => new DashboardVencimentoViewModel
            {
                ContratoId = v.ContratoId,
                Cliente = v.Cliente,
                Descricao = $"{v.Contrato} • {v.Parcela}",
                Valor = FormatarMoeda(v.Valor),
                Vencimento = FormatarVencimento(v.Vencimento),
                CorVencimento = ObterCorVencimento(v)
            })
            .ToList();

        ListaVencimentos.ItemsSource = itens;

        TxtSemVencimentos.Visibility = itens.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PreencherRecebimentos(List<DashboardRecebimentoModel> recebimentos)
    {
        var itens = recebimentos
            .Select(r => new DashboardRecebimentoViewModel
            {
                ContratoId = r.ContratoId,
                Cliente = r.Cliente,
                Contrato = r.Contrato,
                Valor = FormatarMoeda(r.Valor),
                DataPagamento = r.DataPagamento.ToLocalTime().ToString("dd/MM/yyyy HH:mm")
            })
            .ToList();

        ListaRecebimentos.ItemsSource = itens;

        TxtSemRecebimentos.Visibility = itens.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void PreencherContratosRecentes(List<DashboardContratoModel> contratos)
    {
        var itens = contratos
            .Select(c => new DashboardContratoViewModel
            {
                ContratoId = c.ContratoId,
                Cliente = c.Cliente,
                Titulo = c.Titulo,
                ValorTotal = FormatarMoeda(c.ValorTotal),
                Status = c.Status,
                CorStatus = c.Status == StatusContrato.Finalizado.ToString()
                    ? new SolidColorBrush(Color.FromRgb(34, 197, 94))
                    : new SolidColorBrush(Color.FromRgb(56, 189, 248)),
                DataCadastro = c.DataCadastro.ToLocalTime().ToString("dd/MM/yyyy")
            })
            .ToList();

        ListaContratosRecentes.ItemsSource = itens;

        TxtSemContratos.Visibility = itens.Any()
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void AbrirContrato_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement elemento)
            return;

        var contratoId = elemento.DataContext switch
        {
            DashboardVencimentoViewModel vencimento => vencimento.ContratoId,
            DashboardRecebimentoViewModel recebimento => recebimento.ContratoId,
            DashboardContratoViewModel contrato => contrato.ContratoId,
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

    private static string FormatarVencimento(DateTime vencimento)
    {
        var data = vencimento.ToLocalTime().Date;
        var hoje = DateTime.Today;

        if (data < hoje)
        {
            var dias = (hoje - data).Days;

            return dias == 1
                ? "Atrasada há 1 dia"
                : $"Atrasada há {dias} dias";
        }

        if (data == hoje)
            return "Vence hoje";

        if (data == hoje.AddDays(1))
            return "Vence amanhã";

        var diasRestantes = (data - hoje).Days;

        return $"Vence em {diasRestantes} dias";
    }

    private static Brush ObterCorVencimento(DashboardVencimentoModel vencimento)
    {
        if (vencimento.EstaAtrasada)
            return new SolidColorBrush(Color.FromRgb(239, 68, 68));

        if (vencimento.VenceHoje)
            return new SolidColorBrush(Color.FromRgb(250, 204, 21));

        return new SolidColorBrush(Color.FromRgb(148, 163, 184));
    }

    private class DashboardVencimentoViewModel
    {
        public int ContratoId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string Valor { get; set; } = string.Empty;

        public string Vencimento { get; set; } = string.Empty;

        public Brush CorVencimento { get; set; } = Brushes.White;
    }

    private class DashboardRecebimentoViewModel
    {
        public int ContratoId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Contrato { get; set; } = string.Empty;

        public string Valor { get; set; } = string.Empty;

        public string DataPagamento { get; set; } = string.Empty;
    }

    private class DashboardContratoViewModel
    {
        public int ContratoId { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Titulo { get; set; } = string.Empty;

        public string ValorTotal { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public Brush CorStatus { get; set; } = Brushes.White;

        public string DataCadastro { get; set; } = string.Empty;
    }
}