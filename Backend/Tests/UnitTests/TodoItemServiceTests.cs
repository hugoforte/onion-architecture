using AutoFixture;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Shouldly;
using Starter.Contracts;
using Starter.Domain.Entities;
using Starter.Domain.Exceptions;
using Starter.Domain.Repositories;
using Starter.Services;
using Starter.Services.Abstractions;

namespace Starter.UnitTests;

[TestFixture]
public class TodoItemServiceTests
{
    private Fixture _fixture = null!;
    private Mock<ITodoItemRepository> _itemRepository = null!;
    private Mock<ITodoListRepository> _listRepository = null!;
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<IRepositoryManager> _repositoryManager = null!;
    private Mock<INotificationService> _notificationService = null!;
    private TodoItemService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _itemRepository = new Mock<ITodoItemRepository>();
        _listRepository = new Mock<ITodoListRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _notificationService = new Mock<INotificationService>();

        _repositoryManager = new Mock<IRepositoryManager>();
        _repositoryManager.SetupGet(x => x.TodoItems).Returns(_itemRepository.Object);
        _repositoryManager.SetupGet(x => x.TodoLists).Returns(_listRepository.Object);
        _repositoryManager.SetupGet(x => x.UnitOfWork).Returns(_unitOfWork.Object);

        var logger = NullLogger<TodoItemService>.Instance;
        _sut = new TodoItemService(_repositoryManager.Object, _notificationService.Object, logger);
    }

    [Test]
    public async Task CompleteAsync_WhenItemMissing_Throws()
    {
        var missingId = Guid.NewGuid();
        _itemRepository.Setup(r => r.GetByIdAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        await Should.ThrowAsync<TodoItemNotFoundException>(() => _sut.CompleteAsync(missingId));
    }

    [Test]
    public async Task CreateAsync_Persists_and_returns_dto()
    {
        var list = _fixture.Build<TodoList>()
            .Without(l => l.Items)
            .Create();

        _listRepository.Setup(r => r.GetByIdAsync(list.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var dto = new TodoItemForCreationDto
        {
            Title = "Ship template",
            Description = "Finish first cut",
            Priority = TodoPriority.High,
            TodoListId = list.Id
        };

        _itemRepository.Setup(r => r.Insert(It.IsAny<TodoItem>()));
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Title.ShouldBe(dto.Title);
        _itemRepository.Verify(r => r.Insert(It.IsAny<TodoItem>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
