namespace ExaminationSystem.Domain.Entities;

public class Diploma : BaseEntity
{
    private readonly List<Enrollment> _enrollments = [];
    private readonly List<Quiz> _quizzes = [];

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Duration { get; set; }
    public DiplomaStatus Status { get; set; }
    public int QuizCount { get; set; }

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;
    public IReadOnlyCollection<Quiz> Quizzes => _quizzes;
}

