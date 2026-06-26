namespace PagAI.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}