using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Configurations
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired(false);

            builder.Property(x => x.Status)
                .HasConversion<string>();

            builder.Property(x => x.Priority)
                .HasConversion<string>();

            builder.HasOne(x => x.TaskCreator)
                .WithMany(x => x.CreatedTasks)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AssignedTo)
                .WithMany(x => x.AssignedTasks)
                .HasForeignKey(x => x.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Comments)
                .WithOne(x => x.TaskItem)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}