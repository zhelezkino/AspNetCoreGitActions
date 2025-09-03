////////////////////////////////////////////////////////////////////////////////
// See install and settings help in local file:
// AspNetCoreGitActions/Help/InitCommands.cs
////////////////////////////////////////////////////////////////////////////////

using AspNetCoreGitActions.Models;
using AspNetCoreGitActions.Data;
using Microsoft.EntityFrameworkCore;

////////////////////////////////////////////////////////////////////////////////
// Init builder
////////////////////////////////////////////////////////////////////////////////

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Добавляем DbContext с In-Memory для тестов
builder.Services.AddDbContext<TodoContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

////////////////////////////////////////////////////////////////////////////////
// Init app
////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// In-Memory контекст
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<TodoContext>();
context.Database.EnsureCreated();

////////////////////////////////////////////////////////////////////////////////
// Init minimal APIs
////////////////////////////////////////////////////////////////////////////////

// GET: /todos
app.MapGet("/todos", async (TodoContext db) =>
    await db.Todos.ToListAsync())
    .WithName("GetTodos")
    .WithOpenApi();

// GET: /todos/{id}
app.MapGet("/todos/{id}", async (int id, TodoContext db) =>
    await db.Todos.FindAsync(id)
        is TodoItem todo
            ? Results.Ok(todo)
            : Results.NotFound())
    .WithName("GetTodo")
    .WithOpenApi();

// POST: /todos
app.MapPost("/todos", async (TodoItem todo, TodoContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
})
.WithName("CreateTodo")
.WithOpenApi();

// PUT: /todos/{id}
app.MapPut("/todos/{id}", async (int id, TodoItem inputTodo, TodoContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = inputTodo.Title;
    todo.IsCompleted = inputTodo.IsCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateTodo")
.WithOpenApi();

// DELETE: /todos/{id}
app.MapDelete("/todos/{id}", async (int id, TodoContext db) =>
{
    if (await db.Todos.FindAsync(id) is TodoItem todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok();
    }

    return Results.NotFound();
})
.WithName("DeleteTodo")
.WithOpenApi();

////////////////////////////////////////////////////////////////////////////////
// App run
////////////////////////////////////////////////////////////////////////////////

app.Run();
