using System;
using Payments.Domain.Entities;

namespace Payments.Domain.Repositories
{
    public interface IRepositoryManager
    {
        IBillerRepository BillerRepository { get; }

        IGenericRepository<T> GetRepository<T>() where T : BaseEntity;

        IUnitOfWork UnitOfWork { get; }
    }
}
