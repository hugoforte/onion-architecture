using System.Net.Http.Json;
using NUnit.Framework;
using Starter.Contracts;
using Starter.Domain.Entities;
using Shouldly;

namespace Starter.AcceptanceTests;

[TestFixture]
public class TodoApiTests
{
    private TestFixture _fixture = null!;
    private HttpClient _client = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _fixture = new TestFixture();
        await _fixture.InitializeAsync();
        _client = _fixture.CreateClient();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _client.Dispose();
        await _fixture.DisposeAsync();
    }

    [Test]
    public async Task Create_and_get_todo_item()
    {
        var list = await _fixture.CreateListAsync(_client);

        var createItem = new TodoItemForCreationDto
        {
            Title = "Buy milk",
            Description = "Remember oat milk",
            Priority = TodoPriority.High,
            TodoListId = list.Id
        };

        var createResponse = await _client.PostAsJsonAsync($"/api/todolists/{list.Id}/items", createItem);
        createResponse.EnsureSuccessStatusCode();
        var createdItem = await createResponse.Content.ReadFromJsonAsync<TodoItemDto>();

        createdItem.ShouldNotBeNull();
        createdItem.Title.ShouldBe("Buy milk");
        createdItem.TodoListId.ShouldBe(list.Id);

        var getResponse = await _client.GetAsync($"/api/todoitems/{createdItem.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await getResponse.Content.ReadFromJsonAsync<TodoItemDto>();

        fetched.ShouldNotBeNull();
        fetched.Id.ShouldBe(createdItem.Id);
        fetched.Priority.ShouldBe(TodoPriority.High);
    }

    [Test]
    public async Task Complete_todo_item_marks_completed()
    {
        var list = await _fixture.CreateListAsync(_client);

        var createResponse = await _client.PostAsJsonAsync($"/api/todolists/{list.Id}/items", new TodoItemForCreationDto
        {
            Title = "Write docs",
            TodoListId = list.Id
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItemDto>();
        created.ShouldNotBeNull();

        var completeResponse = await _client.PostAsync($"/api/todoitems/{created.Id}/complete", null);
        completeResponse.EnsureSuccessStatusCode();

        var getResponse = await _client.GetAsync($"/api/todoitems/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<TodoItemDto>();

        fetched.ShouldNotBeNull();
        fetched.IsCompleted.ShouldBeTrue();
    }
}
