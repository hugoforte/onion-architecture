namespace Starter.Domain.Repositories;

public interface IRepositoryManager
{
    ITodoListRepository TodoLists { get; }
    ITodoItemRepository TodoItems { get; }
    IUnitOfWork UnitOfWork { get; }
}
