using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class QuestionConfiguration : BaseEntityConfiguration<Question>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.Property(x => x.QuizId).IsRequired();
        builder.Property(x => x.Text)
            .HasMaxLength(2000)
            .IsRequired();
        builder.Property(x => x.Type)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();
        builder.Property(x => x.Explanation)
            .HasMaxLength(4000);
        builder.Property(x => x.OrderIndex).IsRequired();

        builder.HasIndex(x => x.QuizId);

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
    // ?? Quiz 1: C# Fundamentals (Backend Engineering Diploma) ???????????????
    new
    {
        Id = Guid.Parse("E1F2A3B4-C5D6-7890-EFA1-901234567890"),
        QuizId = Guid.Parse("A1111111-1111-1111-1111-111111111111"),
        Text = "Which HTTP method is idempotent and should be used to fully replace a resource?",
        Type = QuestionType.ShortAnswer,
        Explanation = (string?)"PUT is idempotent — calling it multiple times produces the same result.",
        OrderIndex = 1,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    new
    {
        Id = Guid.Parse("F2A3B4C5-D6E7-8901-FAB2-012345678901"),
        QuizId = Guid.Parse("A1111111-1111-1111-1111-111111111111"),
        Text = "What does the SOLID principle 'D' stand for?",
        Type = QuestionType.ShortAnswer,
        Explanation = (string?)"D stands for Dependency Inversion — high-level modules should not depend on low-level modules.",
        OrderIndex = 2,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    new
    {
        Id = Guid.Parse("A3B4C5D6-E7F8-9012-ABC3-123456789012"),
        QuizId = Guid.Parse("A1111111-1111-1111-1111-111111111111"),
        Text = "Which of the following are valid REST constraints?",
        Type = QuestionType.MultipleChoice,
        Explanation = (string?)"Statelessness and uniform interface are both core REST constraints defined by Fielding.",
        OrderIndex = 3,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },

    // ?? Quiz 2: Docker Basics (Cloud & DevOps Diploma) ???????????????????????
    new
    {
        Id = Guid.Parse("B4C5D6E7-F8A9-0123-BCD4-234567890123"),
        QuizId = Guid.Parse("B2222222-2222-2222-2222-222222222222"),
        Text = "What is the primary purpose of a Kubernetes liveness probe?",
        Type = QuestionType.ShortAnswer,
        Explanation = (string?)"A liveness probe tells Kubernetes whether to restart a container.",
        OrderIndex = 1,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    new
    {
        Id = Guid.Parse("C5D6E7F8-A9B0-1234-CDE5-345678901234"),
        QuizId = Guid.Parse("B2222222-2222-2222-2222-222222222222"),
        Text = "Which IaC tool uses a declarative HCL syntax and is cloud-agnostic?",
        Type = QuestionType.ShortAnswer,
        Explanation = (string?)"Terraform by HashiCorp uses HCL and supports multiple cloud providers.",
        OrderIndex = 2,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    new
    {
        Id = Guid.Parse("D6E7F8A9-B0C1-2345-DEF6-456789012345"),
        QuizId = Guid.Parse("B2222222-2222-2222-2222-222222222222"),
        Text = "Which of the following are container orchestration platforms?",
        Type = QuestionType.MultipleChoice,
        Explanation = (string?)"Kubernetes and Docker Swarm are both container orchestration platforms.",
        OrderIndex = 3,
        CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    }
);
    }
}
