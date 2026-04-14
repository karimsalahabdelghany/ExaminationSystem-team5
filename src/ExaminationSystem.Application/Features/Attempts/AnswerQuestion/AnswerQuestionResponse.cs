namespace ExaminationSystem.Application.Features.Attempts.AnswerQuestion;

public record AnswerQuestionResponse(bool Saved, bool TimedOut = false);
