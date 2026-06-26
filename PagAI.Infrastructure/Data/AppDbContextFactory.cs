using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PagAI.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        string connectionString =
                    "Host=aws-1-us-west-2.pooler.supabase.com;" +
                    "Port=5432;" +
                    "Database=postgres;" +
                    "Username=postgres.osypxngekufmduesruex;" +
                    "Password=m23LRqRWUSv2N917;" +
                    "SSL Mode=Require;" +
                    "Trust Server Certificate=true;";     

        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}