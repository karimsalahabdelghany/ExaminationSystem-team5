namespace ExaminationSystem.Domain.Entities;

public class Diploma : BaseEntity
{
    private readonly List<Enrollment> _enrollments = [];
    private readonly List<Quiz> _quizzes = [];

    private Diploma()
    {
    }

    public Diploma(string title,  int duration , int quizCount, string? description = null)
    {
        Title = title;
        Description = description;
        Duration = duration;
        Status = DiplomaStatus.Draft;
        QuizCount = quizCount;
    }

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int Duration { get; private set; }
    public DiplomaStatus Status { get; private set; }
    public int QuizCount { get; private set; }

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;
    public IReadOnlyCollection<Quiz> Quizzes => _quizzes;
}

