using DataAccess.Data;
using DataAccess.Entities;
using Repositories.IRepositories;

namespace Repositories.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Post> Posts { get; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Users = new GenericRepository<User>(_dbContext);
            Posts = new GenericRepository<Post>(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
