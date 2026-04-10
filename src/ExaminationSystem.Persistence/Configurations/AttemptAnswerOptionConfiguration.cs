using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class AttemptAnswerOptionConfiguration : BaseEntityConfiguration<AttemptAnswerOption>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AttemptAnswerOption> builder)
    {
        builder.ToTable("AttemptAnswerOptions");

        builder.Property(x => x.AttemptAnswerId).IsRequired();
        builder.Property(x => x.SelectedOptionId).IsRequired();

        builder.HasIndex(x => x.AttemptAnswerId);
        builder.HasIndex(x => x.SelectedOptionId);

        builder.HasOne(x => x.AttemptAnswer)
            .WithMany(x => x.SelectedOptions)
            .HasForeignKey(x => x.AttemptAnswerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SelectedOption)
            .WithMany(x => x.AttemptAnswerOptions)
            .HasForeignKey(x => x.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
