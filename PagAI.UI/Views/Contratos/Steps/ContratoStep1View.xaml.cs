using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Services;
using PagAI.Domain.Entities;
using PagAI.UI.Components;
using PagAI.UI.Session;

namespace PagAI.UI.Views.Contratos.Steps;

public partial class ContratoStep1View : WizardStep
{
    private readonly ClienteService _clienteService;
    private List<Cliente> _clientes = [];

    public ContratoStep1View()
    {
        InitializeComponent();

        _clienteService = App.ServiceProvider.GetRequiredService<ClienteService>();
    }

    public override async void Carregar()
    {
        await CarregarClientesAsync();

        TxtTitulo.Text = Model.Titulo;
        TxtValor.Text = Model.ValorEmprestado > 0
            ? Model.ValorEmprestado.ToString("N2")
            : string.Empty;

        if (Model.ClienteId > 0)
        {
            CmbClientes.SelectedItem = _clientes
                .FirstOrDefault(c => c.Id == Model.ClienteId);
        }
    }

    private async Task CarregarClientesAsync()
    {
        _clientes = await _clienteService.ListarPorUsuarioAsync(UsuarioSessao.UsuarioId);

        CmbClientes.ItemsSource = _clientes;
    }

    public override void Salvar()
    {
        if (CmbClientes.SelectedItem is Cliente cliente)
            Model.ClienteId = cliente.Id;

        Model.Titulo = TxtTitulo.Text.Trim();

        if (decimal.TryParse(TxtValor.Text, out var valor))
            Model.ValorEmprestado = valor;
    }

    public override bool Validar()
    {
        Salvar();

        if (Model.ClienteId <= 0)
            throw new Exception("Selecione um cliente.");

        if (string.IsNullOrWhiteSpace(Model.Titulo))
            throw new Exception("Informe o título do contrato.");

        if (Model.ValorEmprestado <= 0)
            throw new Exception("Informe um valor emprestado válido.");

        return true;
    }
}