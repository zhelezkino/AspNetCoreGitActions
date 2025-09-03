using AspNetCoreGitActions.Data;
using AspNetCoreGitActions.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreGitActions.Tests;

public class TodoApiTests
{
    private async Task<TodoContext> GetTestContext()
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: $"test_db_{Guid.NewGuid()}")
            .Options;

        var context = new TodoContext(options);
        context.Database.EnsureCreated();

        context.Todos.AddRange(new List<TodoItem>
        {
            new() { Id = 1, Title = "Learn Git", IsCompleted = false },
            new() { Id = 2, Title = "Setup CI/CD", IsCompleted = true }
        });
        await context.SaveChangesAsync();

        return context;
    }

    // Тест метода получения всех элементов (todos), сохраненных в БД
    // Получим подтверждение, что:
    // - данные из БД считываются корректно;
    // - в БД действительно 2 задачи, как задано в GetTestContext();
    // - конкретная задача ("Learn Git") присутствует в списке.
    [Fact]
    public async Task GetTodos_ReturnsAllItems()
    {
        // Arrange
        using var context = await GetTestContext();

        // Act
        var todos = await context.Todos.ToListAsync();

        // Assert
        Assert.Equal(2, todos.Count);
        Assert.Contains(todos, t => t.Title == "Learn Git");
    }

    // Тест возможности добавить новую сущность в БД и сохранить с правильными данными
    // Получим подтверждение, что:
    // - в context создается новый элемент TodoItem;
    // - в БД сохраняются изменения через SaveChangesAsync() и Id сущности был автоматически сгенерирован;
    // - корректно извлекли из БД сохраненную сущность.
    [Fact]
    public async Task AddTodo_SavesToDatabase()
    {
        // Arrange
        using var context = await GetTestContext();
        var newTodo = new TodoItem { Title = "Write tests", IsCompleted = false };

        // Перед сохранением Id должен быть 0 (по умолчанию для int)
        Assert.Equal(0, newTodo.Id); // Убедимся, что Id еще не присвоен

        // Act
        context.Todos.Add(newTodo);
        await context.SaveChangesAsync();

        // Теперь Id должен быть присвоен (не 0)
        Assert.NotEqual(0, newTodo.Id); // Был сгенерирован новый Id

        var saved = await context.Todos.FindAsync(newTodo.Id);

        // Assert
        Assert.NotNull(saved);
        Assert.Equal("Write tests", saved.Title);
    }

    // Тест возможности изменения значений полей сущности в БД
    // Получим подтверждение, что:
    // - можно изменять значение поля сущности в БД;
    // - изменения сохраняются в БД.
    [Fact]
    public async Task UpdateTodo_UpdatesItem()
    {
        // Arrange
        using var context = await GetTestContext();
        var todo = await context.Todos.FirstAsync();

        // Act
        todo.IsCompleted = true;
        const string newTitle = "Updated title";
        todo.Title = newTitle;
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Todos.FindAsync(todo.Id);
        Assert.True(updated?.IsCompleted);
        Assert.Equal(newTitle, updated?.Title);
    }

    // Тест возможности удаления сущности из БД
    // Получим подтверждение, что:
    // - сущность в БД можно удалить через Remove();
    // - после удаления и сохранения, сущность больше не существует в БД.
    [Fact]
    public async Task DeleteTodo_RemovesItem()
    {
        // Arrange
        using var context = await GetTestContext();
        var todo = await context.Todos.FirstAsync();

        // Act
        context.Todos.Remove(todo);
        await context.SaveChangesAsync();

        // Assert
        var exists = await context.Todos.AnyAsync(t => t.Id == todo.Id);
        Assert.False(exists);
    }
}
