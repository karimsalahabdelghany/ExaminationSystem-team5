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

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; }

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;
    public IReadOnlyCollection<Quiz> Quizzes => _quizzes;
}

