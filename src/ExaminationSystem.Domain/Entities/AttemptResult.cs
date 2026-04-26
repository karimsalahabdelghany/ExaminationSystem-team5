namespace ExaminationSystem.Domain.Entities;

public class AttemptResult : BaseEntity
{
    private AttemptResult()
    {
    }

    public AttemptResult(
        Guid attemptId,
        decimal score,
        bool passed,
        int totalQuestions,
        int correctCount,
        DateTime calculatedAt,
        string questionBreakdownJson)
    {
        AttemptId = attemptId;
        Score = score;
        Passed = passed;
        TotalQuestions = totalQuestions;
        CorrectCount = correctCount;
        CalculatedAt = calculatedAt;
        QuestionBreakdownJson = questionBreakdownJson;
    }

    public Guid AttemptId { get; set; }
    public decimal Score { get; set; }
    public bool Passed { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectCount { get; set; }
    public DateTime CalculatedAt { get; set; }
    public string QuestionBreakdownJson { get; set; } = "[]";

    public QuizAttempt Attempt { get; set; } = null!;
}
