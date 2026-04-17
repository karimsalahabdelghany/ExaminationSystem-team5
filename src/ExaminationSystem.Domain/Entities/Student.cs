namespace ExaminationSystem.Domain.Entities;

public class Student : AppUser
{
    private readonly List<Enrollment> _enrollments = [];
    private readonly List<QuizAttempt> _quizAttempts = [];

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments;
    public IReadOnlyCollection<QuizAttempt> QuizAttempts => _quizAttempts;
}
