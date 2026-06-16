using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;

namespace TaskTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)     // Constructor that accepts DbContextOptions and passes it to the base DbContext class
        {
        }

        public DbSet<TaskItem> Tasks { get; set; }                                      // representation of the Task table in the DB

        protected override void OnModelCreating(ModelBuilder modelBuilder)              // configures the table schema
        {
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);                   // defines the Task.Id as the primary key

                entity.Property(t => t.Title)               // configures the Title property to use Task.Title
                    .IsRequired()                           // requires a value for the Title property
                    .HasMaxLength(200);                     // sets a maximum length of 200 characters for the Title property

                entity.Property(t => t.Description)
                    .HasMaxLength(1000);

                entity.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(t => t.IsCompleted)
                    .HasDefaultValue(false);

                entity.Property(t => t.Priority)
                    .HasConversion<string>();
            });
        }
    }
}