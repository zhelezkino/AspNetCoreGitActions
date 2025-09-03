using AspNetCoreGitActions.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreGitActions.Data
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

        public DbSet<TodoItem> Todos { get; set; }

        public DbSet<Car> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            });
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Make).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Year).IsRequired(); // .HasDefaultValue(2000); - for PostgreSQL
                entity.Property(e => e.LicensePlate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsConcurrencyToken(); // Уникальность будет проверяться в API
            });
        }
    }
}
