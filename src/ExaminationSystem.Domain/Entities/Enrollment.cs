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

    public Guid UserId { get; set; }
    public Guid DiplomaId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public EnrollmentStatus Status { get; set; }

    public User User { get; set; } = null!;
    public Diploma Diploma { get; set; } = null!;
}
