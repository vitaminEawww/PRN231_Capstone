﻿using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Feature> Features { get; }

        IGenericRepository<MemFeature> MemFeatures { get; }
        IGenericRepository<Membership> MemberShips { get; }
        Task<int> SaveAsync();
    }
}
