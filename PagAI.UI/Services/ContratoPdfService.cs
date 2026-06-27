using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using PagAI.Domain.Entities;
using PagAI.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PagAI.UI.Services;

public static class ContratoPdfService
{
    public static string GerarComprovanteContrato(Contrato contrato)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var pasta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            "PagAI",
            "Contratos");

        Directory.CreateDirectory(pasta);

        var nomeArquivo = $"pagai-contrato-{contrato.Id}-{DateTime.Now:yyyyMMdd-HHmm}.pdf";
        var caminho = Path.Combine(pasta, nomeArquivo);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => CriarCabecalho(c, contrato));

                page.Content().Column(col =>
                {
                    col.Spacing(14);

                    col.Item().Element(c => CriarCardPrincipal(c, contrato));
                    col.Item().Element(c => CriarCardsResumo(c, contrato));
                    col.Item().Element(c => CriarDadosContrato(c, contrato));
                    col.Item().Element(c => CriarResumoFinanceiro(c, contrato));
                    col.Item().Element(c => CriarTabelaParcelas(c, contrato));
                });

                page.Footer().Element(CriarRodape);
            });
        }).GeneratePdf(caminho);

        return caminho;
    }

    private static void CriarCabecalho(IContainer container, Contrato contrato)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("PagAI")
                        .FontSize(24)
                        .Bold()
                        .FontColor("#2563EB");

                    left.Item().Text("COMPROVANTE FINANCEIRO")
                        .FontSize(9)
                        .SemiBold()
                        .FontColor("#64748B");
                });

                row.RelativeItem().AlignRight().Column(right =>
                {
                    right.Item().AlignRight().Text("DOCUMENTO")
                        .FontSize(8)
                        .SemiBold()
                        .FontColor("#64748B");

                    right.Item().AlignRight().Text($"PAGAI-CRI-{contrato.Id}-{DateTime.Now:yyyyMMdd-HHmm}")
                        .FontSize(10)
                        .Bold()
                        .FontColor("#0F172A");

                    right.Item().PaddingTop(8).AlignRight().Text("GERADO EM")
                        .FontSize(8)
                        .SemiBold()
                        .FontColor("#64748B");

                    right.Item().AlignRight().Text(DateTime.Now.ToString("dd/MM/yyyy, HH:mm"))
                        .FontSize(10)
                        .Bold()
                        .FontColor("#0F172A");
                });
            });

            col.Item().PaddingTop(18).LineHorizontal(1).LineColor("#E2E8F0");
        });
    }

    private static void CriarCardPrincipal(IContainer container, Contrato contrato)
    {
        container
            .Background("#EFF6FF")
            .Border(1)
            .BorderColor("#BFDBFE")
            .CornerRadius(12)
            .Padding(18)
            .Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("CONTRATO CRIADO")
                        .FontSize(9)
                        .Bold()
                        .FontColor("#2563EB");

                    col.Item().PaddingTop(10).Text("Comprovante de criação de contrato")
                        .FontSize(20)
                        .Bold()
                        .FontColor("#0F172A");

                    col.Item().PaddingTop(6).Text("Resumo formal do contrato cadastrado no PagAI para compartilhamento com o cliente.")
                        .FontSize(10)
                        .FontColor("#475569");
                });

                row.ConstantItem(190)
                    .Background(Colors.White)
                    .Border(1)
                    .BorderColor("#DBEAFE")
                    .CornerRadius(10)
                    .Padding(14)
                    .Column(col =>
                    {
                        col.Item().Text("VALOR TOTAL DO CONTRATO")
                            .FontSize(8)
                            .Bold()
                            .FontColor("#64748B");

                        col.Item().PaddingTop(8).Text(FormatarMoeda(contrato.ValorTotalComJuros))
                            .FontSize(20)
                            .Bold()
                            .FontColor("#2563EB");
                    });
            });
    }

    private static void CriarCardsResumo(IContainer container, Contrato contrato)
    {
        container.Row(row =>
        {
            row.RelativeItem().Element(c => CardPequeno(c, "CONTRATO", $"#{contrato.Id}"));
            row.Spacing(10);
            row.RelativeItem().Element(c => CardPequeno(c, "CLIENTE", contrato.Cliente.Nome));
            row.Spacing(10);
            row.RelativeItem().Element(c => CardPequeno(c, "PARCELAS", contrato.QuantidadeParcelas.ToString()));
        });
    }

    private static void CardPequeno(IContainer container, string titulo, string valor)
    {
        container
            .Border(1)
            .BorderColor("#E2E8F0")
            .CornerRadius(10)
            .Padding(12)
            .Column(col =>
            {
                col.Item().Text(titulo)
                    .FontSize(8)
                    .Bold()
                    .FontColor("#64748B");

                col.Item().PaddingTop(6).Text(valor)
                    .FontSize(14)
                    .Bold()
                    .FontColor("#0F172A");
            });
    }

    private static void CriarDadosContrato(IContainer container, Contrato contrato)
    {
        container
            .Border(1)
            .BorderColor("#E2E8F0")
            .CornerRadius(10)
            .Column(col =>
            {
                col.Item()
                    .Background("#F8FAFC")
                    .Padding(10)
                    .Text("Dados do contrato")
                    .FontSize(12)
                    .Bold()
                    .FontColor("#0F172A");

                col.Item().Padding(12).Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text("CLIENTE")
                            .FontSize(9)
                            .Bold()
                            .FontColor("#2563EB");

                        InfoLinha(left, "NOME", contrato.Cliente.Nome);
                        InfoLinha(left, "TELEFONE", contrato.Cliente.Telefone ?? "-");
                        InfoLinha(left, "CPF", contrato.Cliente.CPF ?? "-");
                    });

                    row.RelativeItem().Column(right =>
                    {
                        right.Item().Text("CONTRATO")
                            .FontSize(9)
                            .Bold()
                            .FontColor("#2563EB");

                        InfoLinha(right, "NÚMERO", $"#{contrato.Id}");
                        InfoLinha(right, "DATA DO CONTRATO", contrato.DataContrato.ToLocalTime().ToString("dd/MM/yyyy"));
                        InfoLinha(right, "FORMA DE PAGAMENTO", contrato.TipoPagamento.ToString());
                        InfoLinha(right, "QUANTIDADE DE PARCELAS", contrato.QuantidadeParcelas.ToString());
                        InfoLinha(right, "MÉTODO DE CÁLCULO", contrato.TipoCalculoJuros.ToString());
                        InfoLinha(right, "COBRANÇA DE JUROS", contrato.TipoCobrancaJuros.ToString());
                        InfoLinha(right, "TAXA DE JUROS", $"{contrato.TaxaJuros:0.##}%");
                    });
                });
            });
    }

    private static void InfoLinha(ColumnDescriptor col, string label, string valor)
    {
        col.Item().PaddingTop(8).Row(row =>
        {
            row.ConstantItem(120).Text(label)
                .FontSize(8)
                .Bold()
                .FontColor("#64748B");

            row.RelativeItem().Text(valor)
                .FontSize(10)
                .Bold()
                .FontColor("#0F172A");
        });
    }

    private static void CriarResumoFinanceiro(IContainer container, Contrato contrato)
    {
        var parcelas = contrato.Parcelas.Where(p => p.Ativo).ToList();
        var valorRecebido = parcelas.Sum(p => p.ValorPago);
        var saldoEmAberto = parcelas.Sum(p =>
        {
            var restante = p.Valor - p.ValorPago;
            return restante < 0 ? 0 : restante;
        });

        var jurosPrevistos = contrato.ValorTotalComJuros - contrato.ValorEmprestado;

        container
            .Border(1)
            .BorderColor("#E2E8F0")
            .CornerRadius(10)
            .Column(col =>
            {
                col.Item()
                    .Background("#F8FAFC")
                    .Padding(10)
                    .Text("Resumo financeiro")
                    .FontSize(12)
                    .Bold()
                    .FontColor("#0F172A");

                col.Item().Padding(12).Row(row =>
                {
                    row.RelativeItem().Element(c => FinanceiroItem(c, "VALOR EMPRESTADO", FormatarMoeda(contrato.ValorEmprestado), "#0F172A"));
                    row.RelativeItem().Element(c => FinanceiroItem(c, "VALOR TOTAL", FormatarMoeda(contrato.ValorTotalComJuros), "#0F172A"));
                    row.RelativeItem().Element(c => FinanceiroItem(c, "JUROS PREVISTOS", FormatarMoeda(jurosPrevistos), "#0F172A"));
                    row.RelativeItem().Element(c => FinanceiroItem(c, "VALOR RECEBIDO", FormatarMoeda(valorRecebido), "#059669"));
                    row.RelativeItem().Element(c => FinanceiroItem(c, "SALDO EM ABERTO", FormatarMoeda(saldoEmAberto), "#D97706"));
                });
            });
    }

    private static void FinanceiroItem(IContainer container, string titulo, string valor, string cor)
    {
        container.Column(col =>
        {
            col.Item().Text(titulo)
                .FontSize(8)
                .Bold()
                .FontColor("#64748B");

            col.Item().PaddingTop(6).Text(valor)
                .FontSize(10)
                .Bold()
                .FontColor(cor);
        });
    }

    private static void CriarTabelaParcelas(IContainer container, Contrato contrato)
    {
        var parcelas = contrato.Parcelas
            .Where(p => p.Ativo)
            .OrderBy(p => p.Numero)
            .ToList();

        container
            .Border(1)
            .BorderColor("#E2E8F0")
            .CornerRadius(10)
            .Column(col =>
            {
                col.Item()
                    .Background("#F8FAFC")
                    .Padding(10)
                    .Row(row =>
                    {
                        row.RelativeItem().Text("Parcelas")
                            .FontSize(12)
                            .Bold()
                            .FontColor("#0F172A");

                        row.ConstantItem(100).AlignRight().Text($"{parcelas.Count} registro(s)")
                            .FontSize(9)
                            .Bold()
                            .FontColor("#64748B");
                    });

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(60);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    HeaderTabela(table, "PARCELA");
                    HeaderTabela(table, "VENCIMENTO");
                    HeaderTabela(table, "VALOR");
                    HeaderTabela(table, "RECEBIDO");
                    HeaderTabela(table, "STATUS");

                    foreach (var parcela in parcelas)
                    {
                        CellTabela(table, parcela.Numero.ToString());
                        CellTabela(table, parcela.DataVencimento.ToLocalTime().ToString("dd/MM/yyyy"));
                        CellTabela(table, FormatarMoeda(parcela.Valor));
                        CellTabela(table, FormatarMoeda(parcela.ValorPago));
                        CellTabela(table, ObterStatusParcela(parcela.Status));
                    }
                });
            });
    }

    private static void HeaderTabela(TableDescriptor table, string texto)
    {
        table.Cell()
            .Background("#F8FAFC")
            .Padding(8)
            .Text(texto)
            .FontSize(8)
            .Bold()
            .FontColor("#64748B");
    }

    private static void CellTabela(TableDescriptor table, string texto)
    {
        table.Cell()
            .BorderTop(1)
            .BorderColor("#E2E8F0")
            .Padding(8)
            .Text(texto)
            .FontSize(9)
            .FontColor("#0F172A");
    }

    private static void CriarRodape(IContainer container)
    {
        container.PaddingTop(12).Row(row =>
        {
            row.RelativeItem().Text("Documento gerado pelo PagAI a partir dos registros do contrato. Confira os valores com os comprovantes e documentos originais quando necessário.")
                .FontSize(8)
                .FontColor("#64748B");

            row.ConstantItem(60).AlignRight().Text("PagAI")
                .FontSize(9)
                .Bold()
                .FontColor("#2563EB");
        });
    }

    private static string ObterStatusParcela(StatusParcela status)
    {
        return status switch
        {
            StatusParcela.Pago => "Pago",
            StatusParcela.Atrasado => "Atrasado",
            _ => "Em aberto"
        };
    }

    private static string FormatarMoeda(decimal valor)
    {
        return valor.ToString("C", new CultureInfo("pt-BR"));
    }
}