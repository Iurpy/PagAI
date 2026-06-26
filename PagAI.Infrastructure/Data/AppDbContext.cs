using Microsoft.EntityFrameworkCore;
using PagAI.Domain.Entities;

namespace PagAI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Cliente> Clientes { get; set; }

    public DbSet<Contrato> Contratos { get; set; }

    public DbSet<Parcela> Parcelas { get; set; }

    public DbSet<Pagamento> Pagamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Clientes)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cliente>()
            .HasMany(c => c.Contratos)
            .WithOne(c => c.Cliente)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contrato>()
            .HasMany(c => c.Parcelas)
            .WithOne(p => p.Contrato)
            .HasForeignKey(p => p.ContratoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Valor)
                .HasColumnType("numeric(18,2)");

            entity.HasOne(p => p.Parcela)
                .WithMany(p => p.Pagamentos)
                .HasForeignKey(p => p.ParcelaId)
                .OnDelete(DeleteBehavior.Cascade);
        });    
    }
}