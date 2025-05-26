using DataAccess.Data;
using DataAccess.Entities;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Feature> Features { get; }

        public IGenericRepository<MemFeature> MemFeatures { get; }

        public IGenericRepository<Membership> MemberShips { get; }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Users = new GenericRepository<User>(_dbContext);
            Features = new GenericRepository<Feature>(_dbContext);
            MemFeatures = new GenericRepository<MemFeature>(_dbContext);
            MemberShips = new GenericRepository<Membership>(_dbContext);
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
