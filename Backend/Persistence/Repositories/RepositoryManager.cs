using System;
using System.Collections.Concurrent;
using Payments.Domain.Entities;
using Payments.Domain.Repositories;

namespace Payments.Persistence.Repositories
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly ConcurrentDictionary<Type, object> _repositories;
        private readonly RepositoryDbContext _dbContext;

        public RepositoryManager(IBillerRepository billerRepository, IUnitOfWork unitOfWork, RepositoryDbContext dbContext)
        {
            BillerRepository = billerRepository;
            UnitOfWork = unitOfWork;
            _dbContext = dbContext;
            _repositories = new ConcurrentDictionary<Type, object>();
        }

        public IBillerRepository BillerRepository { get; }

        public IGenericRepository<T> GetRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            return (IGenericRepository<T>)_repositories.GetOrAdd(type, _ => new GenericRepository<T>(_dbContext));
        }

        public IUnitOfWork UnitOfWork { get; }
    }
}
