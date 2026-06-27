using PagAI.Application.Calculators;
using PagAI.Application.Interfaces;
using PagAI.Application.Models;
using PagAI.Domain.Entities;
using PagAI.Domain.Enums;

namespace PagAI.Application.Services;

public class ContratoService
{
    private readonly IContratoRepository _contratoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ContratoCalculator _contratoCalculator;

    public ContratoService(
        IContratoRepository contratoRepository,
        IUnitOfWork unitOfWork,
        ContratoCalculator contratoCalculator)
    {
        _contratoRepository = contratoRepository;
        _unitOfWork = unitOfWork;
        _contratoCalculator = contratoCalculator;
    }

    public async Task CadastrarAsync(ContratoWizardModel model)
    {
        var resumo = _contratoCalculator.GerarResumo(model);

        var contrato = new Contrato
        {
            ClienteId = model.ClienteId,
            Titulo = model.Titulo,
            ValorEmprestado = model.ValorEmprestado,
            ValorTotalComJuros = resumo.ValorTotalComJuros,
            DataContrato = DateTime.UtcNow,
            PrimeiroVencimento = DateTime.SpecifyKind(model.PrimeiroVencimento, DateTimeKind.Utc),
            TipoPagamento = model.TipoPagamento,
            QuantidadeParcelas = model.QuantidadeParcelas,
            Periodicidade = model.Periodicidade,
            TipoCalculoJuros = model.TipoCalculoJuros,
            TipoCobrancaJuros = model.TipoCobrancaJuros,
            TaxaJuros = model.TaxaJuros,
            TipoMulta = model.TipoMulta,
            TaxaMulta = model.TaxaMulta,
            Observacao = model.Observacao,
            Status = StatusContrato.Ativo,
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            Parcelas = resumo.Parcelas.Select(p => new Parcela
{
    Numero = p.Numero,
    Valor = p.Valor,
    DataVencimento = DateTime.SpecifyKind(p.DataVencimento, DateTimeKind.Utc),
    Status = StatusParcela.Pendente,
    Ativo = true,
    DataCadastro = DateTime.UtcNow
}).ToList()
        };

        await CadastrarAsync(contrato);
    }

    public async Task CadastrarAsync(Contrato contrato)
    {
        await _contratoRepository.AdicionarAsync(contrato);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Contrato contrato)
    {
        await _contratoRepository.AtualizarAsync(contrato);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Contrato?> BuscarPorIdAsync(int id)
    {
        return await _contratoRepository.BuscarPorIdAsync(id);
    }

    public async Task<List<Contrato>> ListarPorUsuarioAsync(int usuarioId)
    {
        return await _contratoRepository.ListarPorUsuarioAsync(usuarioId);
    }

    public async Task<List<Contrato>> ListarPorClienteAsync(int clienteId)
    {
        return await _contratoRepository.ListarPorClienteAsync(clienteId);
    }

    public async Task RemoverAsync(Contrato contrato)
    {
        await _contratoRepository.RemoverAsync(contrato);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReceberParcelaAsync(
        int contratoId,
        int parcelaId,
        decimal valorRecebido,
        DateTime dataPagamento,
        string? observacao)
    {
        var contrato = await BuscarPorIdAsync(contratoId);

        if (contrato == null)
            throw new Exception("Contrato não encontrado.");

        var parcela = contrato.Parcelas
            .FirstOrDefault(p => p.Id == parcelaId && p.Ativo);

        if (parcela == null)
            throw new Exception("Parcela não encontrada.");

        if (parcela.Status == StatusParcela.Pago)
            throw new Exception("Essa parcela já está paga.");

        if (valorRecebido <= 0)
            throw new Exception("O valor recebido deve ser maior que zero.");

        var valorRestante = parcela.Valor - parcela.ValorPago;

        if (valorRestante < 0)
            valorRestante = 0;

        if (valorRecebido > valorRestante)
            throw new Exception("O valor recebido ultrapassa o valor restante da parcela.");

        var pagamento = new Pagamento
        {
            ParcelaId = parcela.Id,
            Valor = valorRecebido,
            DataPagamento = DateTime.SpecifyKind(dataPagamento, DateTimeKind.Utc),
            Observacao = observacao,
            DataCadastro = DateTime.UtcNow
        };

        parcela.Pagamentos.Add(pagamento);
        parcela.Observacao = observacao;

        if (parcela.ValorPago >= parcela.Valor)
        {
            parcela.Status = StatusParcela.Pago;
            parcela.DataPagamento = DateTime.SpecifyKind(dataPagamento, DateTimeKind.Utc);
        }
        else
        {
            parcela.Status = StatusParcela.Pendente;
            parcela.DataPagamento = null;
        }

        AtualizarStatusContrato(contrato);

        await _contratoRepository.AtualizarAsync(contrato);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task QuitarContratoAsync(
        int contratoId,
        DateTime dataPagamento)
    {
        var contrato = await BuscarPorIdAsync(contratoId);

        if (contrato == null)
            throw new Exception("Contrato não encontrado.");

        var parcelasPendentes = contrato.Parcelas
            .Where(p => p.Ativo && p.Status != StatusParcela.Pago)
            .ToList();

        if (!parcelasPendentes.Any())
            throw new Exception("Este contrato já está quitado.");

        foreach (var parcela in parcelasPendentes)
        {
            var valorRestante = parcela.Valor - parcela.ValorPago;

            if (valorRestante < 0)
                valorRestante = 0;

            if (valorRestante <= 0)
                continue;

            var pagamento = new Pagamento
            {
                ParcelaId = parcela.Id,
                Valor = valorRestante,
                DataPagamento = DateTime.SpecifyKind(dataPagamento, DateTimeKind.Utc),
                Observacao = "Quitação do contrato",
                DataCadastro = DateTime.UtcNow
            };

            parcela.Pagamentos.Add(pagamento);
            parcela.DataPagamento = DateTime.SpecifyKind(dataPagamento, DateTimeKind.Utc);
            parcela.Status = StatusParcela.Pago;
        }

        AtualizarStatusContrato(contrato);

        await _contratoRepository.AtualizarAsync(contrato);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AtualizarParcelasAtrasadasAsync(int contratoId)
    {
        var contrato = await BuscarPorIdAsync(contratoId);

        if (contrato == null)
            throw new Exception("Contrato não encontrado.");

        var hoje = DateTime.UtcNow.Date;

        foreach (var parcela in contrato.Parcelas.Where(p => p.Ativo))
        {
            if (parcela.Status == StatusParcela.Pago)
                continue;

            var vencimento = parcela.DataVencimento.ToUniversalTime().Date;

            parcela.Status = vencimento < hoje
                ? StatusParcela.Atrasado
                : StatusParcela.Pendente;
        }

        AtualizarStatusContrato(contrato);

        await _contratoRepository.AtualizarAsync(contrato);
        await _unitOfWork.SaveChangesAsync();
    }

    private static void AtualizarStatusContrato(Contrato contrato)
    {
        var parcelasAtivas = contrato.Parcelas
            .Where(p => p.Ativo)
            .ToList();

        contrato.Status = parcelasAtivas.All(p => p.Status == StatusParcela.Pago)
            ? StatusContrato.Finalizado
            : StatusContrato.Ativo;
    }
    
    public async Task<DashboardModel> ObterDashboardAsync(int usuarioId)
{
    var contratos = await _contratoRepository.ListarPorUsuarioAsync(usuarioId);

    var contratosAtivos = contratos
        .Where(c => c.Ativo)
        .ToList();

    var parcelasAtivas = contratosAtivos
        .SelectMany(c => c.Parcelas)
        .Where(p => p.Ativo)
        .ToList();

    var pagamentos = parcelasAtivas
        .SelectMany(p => p.Pagamentos)
        .ToList();

    var hoje = DateTime.UtcNow.Date;

    var capitalEmprestado = parcelasAtivas.Sum(p =>
{
    var restante = p.Valor - p.ValorPago;
    return restante < 0 ? 0 : restante;
});

    var totalRecebido = pagamentos.Sum(p => p.Valor);

    var totalEmAberto = parcelasAtivas.Sum(p =>
    {
        var restante = p.Valor - p.ValorPago;
        return restante < 0 ? 0 : restante;
    });

    var proximosVencimentos = contratosAtivos
        .SelectMany(c => c.Parcelas
            .Where(p => p.Ativo && p.Status != StatusParcela.Pago)
            .Select(p => new DashboardVencimentoModel
            {
                ContratoId = c.Id,
                Cliente = c.Cliente.Nome,
                Contrato = c.Titulo,
                Parcela = $"Parcela {p.Numero}",
                Valor = p.Valor - p.ValorPago < 0 ? 0 : p.Valor - p.ValorPago,
                Vencimento = p.DataVencimento,
                EstaAtrasada = p.DataVencimento.Date < hoje,
                VenceHoje = p.DataVencimento.Date == hoje
            }))
        .OrderBy(x => x.Vencimento)
        .Take(6)
        .ToList();

    var ultimosRecebimentos = contratosAtivos
        .SelectMany(c => c.Parcelas
            .SelectMany(p => p.Pagamentos
                .Select(pg => new DashboardRecebimentoModel
                {
                    ContratoId = c.Id,
                    Cliente = c.Cliente.Nome,
                    Contrato = c.Titulo,
                    Valor = pg.Valor,
                    DataPagamento = pg.DataPagamento
                })))
        .OrderByDescending(x => x.DataPagamento)
        .Take(6)
        .ToList();

    var contratosRecentes = contratosAtivos
        .OrderByDescending(c => c.DataCadastro)
        .Take(6)
        .Select(c => new DashboardContratoModel
        {
            ContratoId = c.Id,
            Cliente = c.Cliente.Nome,
            Titulo = c.Titulo,
            ValorTotal = c.ValorTotalComJuros,
            Status = c.Status.ToString(),
            DataCadastro = c.DataCadastro
        })
        .ToList();

    return new DashboardModel
    {
        CapitalEmprestado = capitalEmprestado,
        TotalRecebido = totalRecebido,
        TotalEmAberto = totalEmAberto,
        ContratosAtivos = contratosAtivos.Count(c => c.Status == StatusContrato.Ativo),
        ContratosFinalizados = contratosAtivos.Count(c => c.Status == StatusContrato.Finalizado),
        ParcelasAtrasadas = parcelasAtivas.Count(p => p.Status == StatusParcela.Atrasado),
        ParcelasVencendoHoje = parcelasAtivas.Count(p =>
            p.Status != StatusParcela.Pago &&
            p.DataVencimento.Date == hoje),
        ClientesComContrato = contratosAtivos
            .Select(c => c.ClienteId)
            .Distinct()
            .Count(),
        ProximosVencimentos = proximosVencimentos,
        UltimosRecebimentos = ultimosRecebimentos,
        ContratosRecentes = contratosRecentes
    };
}

public async Task<AgendaCobrancasModel> ObterAgendaCobrancasAsync(int usuarioId)
{
    var contratos = await _contratoRepository.ListarPorUsuarioAsync(usuarioId);

    var hoje = DateTime.UtcNow.Date;
    var limite = hoje.AddDays(7);

    var parcelas = contratos
        .Where(c => c.Ativo && c.Status == StatusContrato.Ativo)
        .SelectMany(c => c.Parcelas
            .Where(p =>
                p.Ativo &&
                p.Status != StatusParcela.Pago)
            .Select(p =>
            {
                var valorRestante = p.Valor - p.ValorPago;

                if (valorRestante < 0)
                    valorRestante = 0;

                var vencimento = p.DataVencimento.Date;

                return new AgendaCobrancaItemModel
                {
                    ContratoId = c.Id,
                    ParcelaId = p.Id,
                    Cliente = c.Cliente.Nome,
                    Contrato = c.Titulo,
                    Parcela = $"Parcela {p.Numero}",
                    ValorRestante = valorRestante,
                    DataVencimento = p.DataVencimento,
                    Dias = vencimento < hoje
                        ? (hoje - vencimento).Days
                        : (vencimento - hoje).Days,
                    Status = vencimento < hoje
                        ? "Atrasada"
                        : vencimento == hoje
                            ? "Vence hoje"
                            : "Próxima"
                };
            }))
        .Where(x => x.ValorRestante > 0)
        .ToList();

    var atrasadas = parcelas
        .Where(x => x.DataVencimento.Date < hoje)
        .OrderByDescending(x => x.Dias)
        .ToList();

    var vencendoHoje = parcelas
        .Where(x => x.DataVencimento.Date == hoje)
        .OrderBy(x => x.Cliente)
        .ToList();

    var proximosDias = parcelas
        .Where(x =>
            x.DataVencimento.Date > hoje &&
            x.DataVencimento.Date <= limite)
        .OrderBy(x => x.DataVencimento)
        .ToList();

    return new AgendaCobrancasModel
    {
        TotalAtrasadas = atrasadas.Count,
        TotalVencendoHoje = vencendoHoje.Count,
        TotalProximosDias = proximosDias.Count,

        ValorAtrasado = atrasadas.Sum(x => x.ValorRestante),
        ValorVencendoHoje = vencendoHoje.Sum(x => x.ValorRestante),
        ValorProximosDias = proximosDias.Sum(x => x.ValorRestante),

        Atrasadas = atrasadas,
        VencendoHoje = vencendoHoje,
        ProximosDias = proximosDias
    };
}

public async Task<RelatorioModel> ObterRelatorioAsync(int usuarioId)
{
    var contratos = await _contratoRepository.ListarPorUsuarioAsync(usuarioId);

    var contratosAtivos = contratos
        .Where(c => c.Ativo)
        .ToList();

    var parcelasAtivas = contratosAtivos
        .SelectMany(c => c.Parcelas)
        .Where(p => p.Ativo)
        .ToList();

    var pagamentos = parcelasAtivas
        .SelectMany(p => p.Pagamentos)
        .ToList();

    var hoje = DateTime.UtcNow.Date;

    var totalEmprestado = contratosAtivos.Sum(c => c.ValorEmprestado);
    var totalComJuros = contratosAtivos.Sum(c => c.ValorTotalComJuros);
    var totalRecebido = pagamentos.Sum(p => p.Valor);

    var totalEmAberto = parcelasAtivas.Sum(p =>
    {
        var restante = p.Valor - p.ValorPago;
        return restante < 0 ? 0 : restante;
    });

    return new RelatorioModel
    {
        TotalEmprestado = totalEmprestado,
        TotalComJuros = totalComJuros,
        TotalRecebido = totalRecebido,
        TotalEmAberto = totalEmAberto,
        LucroPrevisto = totalComJuros - totalEmprestado,

        TotalContratos = contratosAtivos.Count,
        ContratosAtivos = contratosAtivos.Count(c => c.Status == StatusContrato.Ativo),
        ContratosFinalizados = contratosAtivos.Count(c => c.Status == StatusContrato.Finalizado),
        ParcelasAtrasadas = parcelasAtivas.Count(p => p.Status == StatusParcela.Atrasado),

        Contratos = contratosAtivos
            .OrderByDescending(c => c.DataContrato)
            .Select(c =>
            {
                var parcelas = c.Parcelas
                    .Where(p => p.Ativo)
                    .ToList();

                var recebido = parcelas
                    .SelectMany(p => p.Pagamentos)
                    .Sum(p => p.Valor);

                var emAberto = parcelas.Sum(p =>
                {
                    var restante = p.Valor - p.ValorPago;
                    return restante < 0 ? 0 : restante;
                });

                return new RelatorioContratoModel
                {
                    ContratoId = c.Id,
                    Cliente = c.Cliente.Nome,
                    Titulo = c.Titulo,
                    ValorEmprestado = c.ValorEmprestado,
                    ValorTotal = c.ValorTotalComJuros,
                    ValorRecebido = recebido,
                    ValorEmAberto = emAberto,
                    Status = c.Status.ToString(),
                    DataContrato = c.DataContrato
                };
            })
            .ToList(),

        Recebimentos = contratosAtivos
            .SelectMany(c => c.Parcelas
                .Where(p => p.Ativo)
                .SelectMany(p => p.Pagamentos
                    .Select(pg => new RelatorioRecebimentoModel
                    {
                        ContratoId = c.Id,
                        Cliente = c.Cliente.Nome,
                        Contrato = c.Titulo,
                        Parcela = $"Parcela {p.Numero}",
                        Valor = pg.Valor,
                        DataPagamento = pg.DataPagamento,
                        Observacao = pg.Observacao
                    })))
            .OrderByDescending(r => r.DataPagamento)
            .ToList(),

        Inadimplentes = contratosAtivos
            .SelectMany(c => c.Parcelas
                .Where(p =>
                    p.Ativo &&
                    p.Status == StatusParcela.Atrasado &&
                    p.ValorPago < p.Valor)
                .Select(p =>
                {
                    var restante = p.Valor - p.ValorPago;

                    return new RelatorioInadimplenteModel
                    {
                        ContratoId = c.Id,
                        Cliente = c.Cliente.Nome,
                        Contrato = c.Titulo,
                        Parcela = $"Parcela {p.Numero}",
                        ValorRestante = restante < 0 ? 0 : restante,
                        DataVencimento = p.DataVencimento,
                        DiasAtraso = Math.Max(0, (hoje - p.DataVencimento.Date).Days)
                    };
                }))
            .OrderByDescending(i => i.DiasAtraso)
            .ToList()
    };
}
}