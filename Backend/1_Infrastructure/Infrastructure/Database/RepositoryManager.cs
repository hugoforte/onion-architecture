using Starter.Domain.Repositories;
using Starter.Infrastructure.Database.Repositories;

namespace Starter.Infrastructure.Database;

internal sealed class RepositoryManager : IRepositoryManager
{
    public RepositoryManager(ITodoListRepository todoLists, ITodoItemRepository todoItems, IUnitOfWork unitOfWork)
    {
        TodoLists = todoLists;
        TodoItems = todoItems;
        UnitOfWork = unitOfWork;
    }

    public ITodoListRepository TodoLists { get; }
    public ITodoItemRepository TodoItems { get; }
    public IUnitOfWork UnitOfWork { get; }
}
