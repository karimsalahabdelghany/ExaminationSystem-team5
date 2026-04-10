using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class EnrollmentConfiguration : BaseEntityConfiguration<Enrollment>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.DiplomaId).IsRequired();
        builder.Property(x => x.EnrolledAt)
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.DiplomaId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Diploma)
            .WithMany(x => x.Enrollments)
            .HasForeignKey(x => x.DiplomaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
