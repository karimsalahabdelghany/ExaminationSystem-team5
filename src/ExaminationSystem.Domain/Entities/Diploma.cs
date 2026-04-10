namespace ExaminationSystem.Domain.Entities;

public class Diploma : BaseEntity
{
    private readonly List<Enrollment> _enrollments = [];
    private readonly List<Quiz> _quizzes = [];

    private Diploma()
    {
    }

    public Diploma(string name, string description, int duration)
    {
        Name = name;
        Description = description;
        Duration = duration;
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int Duration { get; private set; }

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;
    public IReadOnlyCollection<Quiz> Quizzes => _quizzes;
}

