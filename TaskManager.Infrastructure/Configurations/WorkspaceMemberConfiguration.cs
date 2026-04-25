using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Configurations
{
    public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
    {
        public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
        {
            builder.HasKey(k => new { k.UserId, k.WorkspaceId });

            builder.Property(x => x.Role)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(7);

            builder.HasOne(x => x.AppUser)
                .WithMany(x => x.WorkspaceMembers)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Workspace)
                .WithMany(x => x.WorkspaceMembers)
                .HasForeignKey(x => x.WorkspaceId);
        }
    }
}