using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PagAI.Application.Models;
using PagAI.Application.Services;
using PagAI.UI.Components;
using PagAI.UI.Services;
using PagAI.UI.Views.Contratos.Steps;

namespace PagAI.UI.Views;

public partial class ContratoWizardView : UserControl
{
    private readonly ContratoService _contratoService;

    private readonly ContratoWizardModel _model = new();

    private readonly List<WizardStep> _steps;

    private int _stepAtual;

    public ContratoWizardView()
    {
        InitializeComponent();

        _contratoService = App.ServiceProvider.GetRequiredService<ContratoService>();

        _steps =
        [
            new ContratoStep1View(),
            new ContratoStep2View(),
            new ContratoStep3View(),
            new ContratoStep4View()
        ];

        MostrarStep();
    }

    private void MostrarStep()
    {
        var step = _steps[_stepAtual];

        step.DefinirModel(_model);

        StepContent.Content = step;

        ProgressSteps.Value = _stepAtual + 1;

        BtnVoltar.Content = _stepAtual == 0
            ? "← Contratos"
            : "← Voltar";

        BtnVoltar.IsEnabled = true;

        BtnProximo.Content = _stepAtual == _steps.Count - 1
            ? "Salvar"
            : "Próximo →";
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e)
    {
        if (_stepAtual == 0)
        {
            NavigationService.Navigate(new ContratosView());
            return;
        }

        _steps[_stepAtual].Salvar();

        _stepAtual--;

        MostrarStep();
    }

    private async void BtnProximo_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var step = _steps[_stepAtual];

            if (!step.Validar())
                return;

            step.Salvar();

            if (_stepAtual < _steps.Count - 1)
            {
                _stepAtual++;
                MostrarStep();
                return;
            }

            await SalvarContratoAsync();
        }
        catch (Exception ex)
        {
            var mensagem = ex.InnerException?.Message ?? ex.Message;
            NotificationService.Error(mensagem);
        }
    }

    private async Task SalvarContratoAsync()
    {
        await _contratoService.CadastrarAsync(_model);

        NotificationService.Success("Contrato cadastrado com sucesso.");

        NavigationService.Navigate(new ContratosView());
    }
}