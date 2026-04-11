using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Domain.Entities;

public class Enrollment : BaseEntity
{
    private Enrollment()
    {
    }

    public Enrollment(Guid userId, Guid diplomaId, DateTime enrolledAt, EnrollmentStatus status)
    {
        UserId = userId;
        DiplomaId = diplomaId;
        EnrolledAt = enrolledAt;
        Status = status;
    }

    public Guid UserId { get; private set; }
    public Guid DiplomaId { get; private set; }
    public DateTime EnrolledAt { get; private set; }
    public EnrollmentStatus Status { get; private set; }

    public User User { get; private set; } = null!;
    public Diploma Diploma { get; private set; } = null!;
}

