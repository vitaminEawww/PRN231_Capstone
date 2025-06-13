using System;
using System.Threading.Tasks;
using DataAccess.Entities;

namespace Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Post> Posts { get; }
        Task<int> SaveAsync();
    }
}
