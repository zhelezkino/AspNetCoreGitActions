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

    // ���� ������ ��������� ���� ��������� (todos), ����������� � ��
    // ������� �������������, ���:
    // - ������ �� �� ����������� ���������;
    // - � �� ������������� 2 ������, ��� ������ � GetTestContext();
    // - ���������� ������ ("Learn Git") ������������ � ������.
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

    // ���� ����������� �������� ����� �������� � �� � ��������� � ����������� �������
    // ������� �������������, ���:
    // - � context ��������� ����� ������� TodoItem;
    // - � �� ����������� ��������� ����� SaveChangesAsync() � Id �������� ��� ������������� ������������;
    // - ��������� �������� �� �� ����������� ��������.
    [Fact]
    public async Task AddTodo_SavesToDatabase()
    {
        // Arrange
        using var context = await GetTestContext();
        var newTodo = new TodoItem { Title = "Write tests", IsCompleted = false };

        // ����� ����������� Id ������ ���� 0 (�� ��������� ��� int)
        Assert.Equal(0, newTodo.Id); // ��������, ��� Id ��� �� ��������

        // Act
        context.Todos.Add(newTodo);
        await context.SaveChangesAsync();

        // ������ Id ������ ���� �������� (�� 0)
        Assert.NotEqual(0, newTodo.Id); // ��� ������������ ����� Id

        var saved = await context.Todos.FindAsync(newTodo.Id);

        // Assert
        Assert.NotNull(saved);
        Assert.Equal("Write tests", saved.Title);
    }

    // ���� ����������� ��������� �������� ����� �������� � ��
    // ������� �������������, ���:
    // - ����� �������� �������� ���� �������� � ��;
    // - ��������� ����������� � ��.
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

    // ���� ����������� �������� �������� �� ��
    // ������� �������������, ���:
    // - �������� � �� ����� ������� ����� Remove();
    // - ����� �������� � ����������, �������� ������ �� ���������� � ��.
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
