using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class AttemptResultConfiguration : IEntityTypeConfiguration<AttemptResult>
{
    public void Configure(EntityTypeBuilder<AttemptResult> builder)
    {
        builder.ToTable("AttemptResults");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(x => x.AttemptId).IsRequired();
        builder.Property(x => x.Score)
            .HasColumnType("decimal(9,2)")
            .IsRequired();
        builder.Property(x => x.Passed).IsRequired();
        builder.Property(x => x.TotalQuestions).IsRequired();
        builder.Property(x => x.CorrectCount).IsRequired();
        builder.Property(x => x.CalculatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(x => x.AttemptId).IsUnique();

        builder.HasQueryFilter(x => !x.Attempt.IsDeleted);
        
    }
}
