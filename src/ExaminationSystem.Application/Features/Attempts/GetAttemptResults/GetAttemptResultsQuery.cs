using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Application.Features.Attempts.GetAttemptResults;

public record GetAttemptResultsQuery(
    Guid AttemptId,
    Guid? RequesterId,
    bool IsAdmin
) : IQuery<RequestResult<GetAttemptResultsResponse>>;

public class GetAttemptResultsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAttemptResultsQuery, RequestResult<GetAttemptResultsResponse>>
{
    public async Task<RequestResult<GetAttemptResultsResponse>> Handle(
        GetAttemptResultsQuery request,
        CancellationToken cancellationToken)
    {
        var attemptRepository = unitOfWork.Repository<QuizAttempt>();
        var resultRepository = unitOfWork.Repository<AttemptResult>();
        var answerRepository = unitOfWork.Repository<AttemptAnswer>();
        var questionRepository = unitOfWork.Repository<Question>();

        var attempt = await attemptRepository
            .GetAll(a => a.Id == request.AttemptId)
            .Select(a => new
            {
                a.Id,
                a.UserId,
                a.QuizId,
                a.Status
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (attempt is null)
            return RequestResult<GetAttemptResultsResponse>.Failure(null!, ResultCode.AttemptNotFound);

        if (!request.IsAdmin && (!request.RequesterId.HasValue || attempt.UserId != request.RequesterId.Value))
            return RequestResult<GetAttemptResultsResponse>.Failure(null!, ResultCode.AttemptNotOwned);

        if (attempt.Status is QuizAttemptStatus.InProgress or QuizAttemptStatus.NotStarted or QuizAttemptStatus.Submitting)
            return RequestResult<GetAttemptResultsResponse>.Failure(null!, ResultCode.AttemptResultsNotAvailableYet);

        var attemptResult = await resultRepository
            .GetAll(r => r.AttemptId == request.AttemptId)
            .Select(r => new
            {
                r.Score,
                r.Passed,
                r.TotalQuestions,
                r.CorrectCount
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (attemptResult is null)
            return RequestResult<GetAttemptResultsResponse>.Failure(null!, ResultCode.AttemptResultsNotFound);

        var answeredLookup = await answerRepository
            .GetAll(a => a.AttemptId == request.AttemptId)
            .OrderByDescending(a => a.AnsweredAt)
            .Select(a => new
            {
                a.QuestionId,
                SelectedOptionText = a.SelectedOption.Text,
                IsCorrect = a.SelectedOption.IsCorrect
            })
            .GroupBy(a => a.QuestionId)
            .Select(group => group.First())
            .ToDictionaryAsync(a => a.QuestionId, cancellationToken);

        var questions = await questionRepository
            .GetAll(q => q.QuizId == attempt.QuizId)
            .OrderBy(q => q.OrderIndex)
            .Select(q => new
            {
                q.Id,
                q.Explanation,
                CorrectOptions = q.Options
                    .Where(option => option.IsCorrect)
                    .OrderBy(option => option.OrderIndex)
                    .Select(option => option.Text)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var perQuestion = questions
            .Select(question =>
            {
                var hasStudentAnswer = answeredLookup.TryGetValue(question.Id, out var answer);
                return new AttemptQuestionResultResponse(
                    QuestionId: question.Id,
                    StudentAnswer: hasStudentAnswer ? answer!.SelectedOptionText : null,
                    CorrectAnswer: question.CorrectOptions.Count == 0 ? null : string.Join(", ", question.CorrectOptions),
                    IsCorrect: hasStudentAnswer && answer!.IsCorrect,
                    Explanation: question.Explanation
                );
            })
            .ToList();

        var response = new GetAttemptResultsResponse(
            Score: attemptResult.Score,
            Passed: attemptResult.Passed,
            TotalQuestions: attemptResult.TotalQuestions,
            CorrectCount: attemptResult.CorrectCount,
            PerQuestion: perQuestion
        );

        return RequestResult<GetAttemptResultsResponse>.succeeded(response, ResultCode.AttemptResultsRetrievedSuccessfully);
    }
}
