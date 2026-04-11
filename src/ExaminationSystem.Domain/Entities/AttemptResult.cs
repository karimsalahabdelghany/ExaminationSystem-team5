namespace ExaminationSystem.Domain.Entities;

public class AttemptResult
{
    private AttemptResult()
    {
    }

    public AttemptResult(Guid attemptId, int score, int totalQuestions, int correctAnswers, float percentage)
    {
        AttemptId = attemptId;
        Score = score;
        TotalQuestions = totalQuestions;
        CorrectAnswers = correctAnswers;
        Percentage = percentage;
    }

    public Guid AttemptId { get; private set; }
    public int Score { get; private set; }
    public int TotalQuestions { get; private set; }
    public int CorrectAnswers { get; private set; }
    public float Percentage { get; private set; }

    public QuizAttempt Attempt { get; private set; } = null!;
}

