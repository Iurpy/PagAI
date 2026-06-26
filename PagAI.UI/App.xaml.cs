using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PagAI.Infrastructure.Data;
using PagAI.Application.Interfaces;
using PagAI.Application.Services;
using PagAI.Infrastructure.Repositories;
using PagAI.Application.Calculators;

namespace PagAI.UI;

public partial class App : System.Windows.Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();

        string connectionString =
            "Host=aws-1-us-west-2.pooler.supabase.com;" +
            "Port=5432;" +
            "Database=postgres;" +
            "Username=postgres.osypxngekufmduesruex;" +
            "Password=m23LRqRWUSv2N917;" +
            "SSL Mode=Require;" +
            "Trust Server Certificate=true;";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IContratoRepository, ContratoRepository>();
        services.AddScoped<UsuarioService>();
        services.AddScoped<ClienteService>();
        services.AddScoped<ContratoService>();
        services.AddScoped<ContratoCalculator>();



        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = new MainWindow();
        mainWindow.Show();

        base.OnStartup(e);
    }
}