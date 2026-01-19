using BatchProcessingQuartzPoc.Domain;
using Microsoft.EntityFrameworkCore;

namespace BatchProcessingQuartzPoc.Infrastructure.Persistence;

public class BatchDbContext : DbContext
{
    public BatchDbContext(DbContextOptions<BatchDbContext> options) : base(options)
    {
    }

    public DbSet<BatchAction> BatchActions => Set<BatchAction>();
    public DbSet<BatchActionItem> BatchActionItems => Set<BatchActionItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BatchAction>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.ActionType).IsRequired();
            entity.Property(a => a.Status).IsRequired();
            entity.Property(a => a.CreatedAtUtc).IsRequired();
            entity.HasMany(a => a.Items)
                .WithOne(i => i.BatchAction!)
                .HasForeignKey(i => i.BatchActionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BatchActionItem>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ItemReference).IsRequired();
            entity.Property(i => i.Status).IsRequired();
            entity.Property(i => i.CreatedAtUtc).IsRequired();
        });
    }
}
