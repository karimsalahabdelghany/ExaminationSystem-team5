using ExaminationSystem.Application.Common.Results;


namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminAttemptDetailsQuery(Guid AttemptId)
        : IRequest<RequestResult<GetAdminAttemptDetailsResponse>>;

    
    public class GetAdminAttemptDetailsQueryHandler
        : IRequestHandler<GetAdminAttemptDetailsQuery, RequestResult<GetAdminAttemptDetailsResponse>>
    {
        private readonly IRepository<QuizAttempt> _quizAttemptrepo;

        public GetAdminAttemptDetailsQueryHandler(IRepository<QuizAttempt> QuizAttemptrepo)
        {
            _quizAttemptrepo = QuizAttemptrepo;
        }

        public async Task<RequestResult<GetAdminAttemptDetailsResponse>> Handle(
            GetAdminAttemptDetailsQuery request,
            CancellationToken cancellationToken)
        {
            //student, quiz (+ questions + options), result, answers
            var attempt = await _quizAttemptrepo.GetAll(a => a.Id == request.AttemptId)
                .Include(a => a.Student)
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(q => q.Options)
                .Include(a => a.Result)
                .Include(a => a.Answers)
                .FirstOrDefaultAsync(cancellationToken);

            if (attempt is null)
                return RequestResult<GetAdminAttemptDetailsResponse>.Failure(null, ResultCode.AttemptNotFound);

            // in-memory after the single DB call
            var answersByQuestion = attempt.Answers.ToDictionary(ans => ans.QuestionId);

            var perQuestion = attempt.Quiz.Questions
                .OrderBy(q => q.OrderIndex)
                .Select(q =>
                {
                    var correctOption = q.Options.FirstOrDefault(o => o.IsCorrect);
                    var hasAnswer = answersByQuestion.TryGetValue(q.Id, out var answer);
                    var selectedOption = hasAnswer
                        ? q.Options.FirstOrDefault(o => o.Id == answer!.SelectedOptionId)
                        : null;

                    return new QuestionBreakdownItem(
                        QuestionId: q.Id,
                        QuestionText: q.Text,
                        StudentAnswer: selectedOption?.Text,
                        CorrectAnswer: correctOption?.Text ?? string.Empty,
                        IsCorrect: selectedOption?.IsCorrect ?? false
                    );
                })
                .ToList();

            var response = new GetAdminAttemptDetailsResponse(
                AttemptId: attempt.Id,
                StudentId: attempt.UserId,
                StudentName: attempt.Student.FullName,
                QuizId: attempt.QuizId,
                QuizTitle: attempt.Quiz.Title,
                Status: attempt.Status,
                StartTime: attempt.StartTime,
                Deadline: attempt.Deadline,
                SubmittedAt: attempt.SubmittedAt,
                Score: attempt.Result != null ? attempt.Result.Score : null,
                Passed: attempt.Result != null ? attempt.Result.Passed : null,
                TotalQuestions: attempt.Result != null ? attempt.Result.TotalQuestions : null,
                CorrectCount: attempt.Result != null ? attempt.Result.CorrectCount : null,
                PerQuestion: perQuestion
            );

            return RequestResult<GetAdminAttemptDetailsResponse>.succeeded(response, ResultCode.AdminAttemptDetailsRetrievedSuccessfully);
        }
    }
}
