
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Body)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasOne(x => x.Writer)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.WriterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}