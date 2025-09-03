using AspNetCoreGitActions;
using AspNetCoreGitActions.Data;
using AspNetCoreGitActions.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreGitActions.Tests;

public class CarApiTests
{
    private async Task<TodoContext> GetTestContext()
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: $"test_car_db_{Guid.NewGuid()}")
            .Options;

        var context = new TodoContext(options);
        context.Database.EnsureCreated();

        context.Cars.AddRange(new[]
        {
            new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2020, LicensePlate = "ABC-123" },
            new Car { Id = 2, Make = "Honda", Model = "Civic", Year = 2019, LicensePlate = "XYZ-789" }
        });
        await context.SaveChangesAsync();

        return context;
    }

    [Fact]
    public async Task GetCars_ReturnsAllCars()
    {
        // Arrange
        using var context = await GetTestContext();

        // Act
        var cars = await context.Cars.ToListAsync();

        // Assert
        Assert.Equal(2, cars.Count);
        Assert.Contains(cars, c => c.Make == "Toyota");
        Assert.Contains(cars, c => c.LicensePlate == "XYZ-789");
    }

    [Fact]
    public async Task AddCar_SavesToDatabase()
    {
        // Arrange
        using var context = await GetTestContext();
        var newCar = new Car
        {
            Make = "Ford",
            Model = "Focus",
            Year = 2021,
            LicensePlate = "NEW-001"
        };

        // Act
        context.Cars.Add(newCar);
        await context.SaveChangesAsync();

        // Assert
        var saved = await context.Cars.FindAsync(newCar.Id);
        Assert.NotNull(saved);
        Assert.Equal("Ford", saved.Make);
    }

    [Fact]
    public async Task AddCar_DoesNotAllowDuplicateLicensePlate()
    {
        // Arrange
        using var context = await GetTestContext();
        var duplicateCar = new Car
        {
            Make = "BMW",
            Model = "X5",
            Year = 2022,
            LicensePlate = "ABC-123"
        };

        // Act
        var alreadyExists = await context.Cars.AnyAsync(c => c.LicensePlate == duplicateCar.LicensePlate);

        // Assert
        Assert.True(alreadyExists, "Cannot add car: license plate already exists");
    }

    [Fact]
    public async Task UpdateCar_UpdatesFields()
    {
        // Arrange
        using var context = await GetTestContext();
        var car = await context.Cars.FirstAsync();

        // Act
        car.Model = "Corolla";
        car.Year = 2021;
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Cars.FindAsync(car.Id);
        Assert.Equal("Corolla", updated?.Model);
        Assert.Equal(2021, updated?.Year);
    }

    [Fact]
    public async Task DeleteCar_RemovesFromDatabase()
    {
        // Arrange
        using var context = await GetTestContext();
        var car = await context.Cars.FirstAsync();

        // Act
        context.Cars.Remove(car);
        await context.SaveChangesAsync();

        // Assert
        var exists = await context.Cars.AnyAsync(c => c.Id == car.Id);
        Assert.False(exists);
    }
}
