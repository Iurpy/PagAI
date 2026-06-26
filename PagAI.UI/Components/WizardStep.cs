using System.Windows.Controls;
using PagAI.Application.Models;

namespace PagAI.UI.Components;

public abstract class WizardStep : UserControl
{
    public ContratoWizardModel Model { get; private set; } = null!;

    public void DefinirModel(ContratoWizardModel model)
    {
        Model = model;
        Carregar();
    }

    public virtual void Carregar()
    {
    }

    public virtual void Salvar()
    {
    }

    public virtual bool Validar()
    {
        return true;
    }
}