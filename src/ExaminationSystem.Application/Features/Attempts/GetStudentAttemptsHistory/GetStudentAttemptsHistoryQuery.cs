//using ExaminationSystem.Application.Common.Helper.Pagination;
//using ExaminationSystem.Application.Common.Results;
//using LinqKit;

//namespace ExaminationSystem.Application.Features.Attempts.GetStudentAttemptsHistory;

//public record GetStudentAttemptsHistoryQuery
//(Guid? QuizId , Guid? DiplomaId , int? PageNumber , int? PageSize , Guid StudentId) 
//: IQuery<RequestResult<PaginationResult<GetStudentAttemptHistoryResponse>>>;

//public class GetStudentAttemptsHistoryQueryHandler(IRepository<QuizAttempt> _repository
//            ,ILogger<GetStudentAttemptsHistoryQueryHandler> _logger)
//    : IRequestHandler<GetStudentAttemptsHistoryQuery, RequestResult<PaginationResult<GetStudentAttemptHistoryResponse>>>
//{

//    public async Task<RequestResult<PaginationResult<GetStudentAttemptHistoryResponse>>> Handle(GetStudentAttemptsHistoryQuery request, CancellationToken cancellationToken)
//    {
//        _logger.LogInformation(
//          "Fetching attempt history for Student {StudentId} | DiplomaId: {DiplomaId}, QuizId: {QuizId}, Page: {Page}/{Size}",
//          request.StudentId, request.DiplomaId, request.QuizId, request.PageNumber, request.PageSize);

//        var query = _repository.GetAll(qa => qa.UserId == request.StudentId)
//                               .Where (qa =>  !request.QuizId.HasValue  || qa.QuizId == request.QuizId.Value)
//                               .Where(qa => !request.DiplomaId.HasValue || qa.Quiz.DiplomaId == request.DiplomaId.Value)
//                               .OrderByDescending(qa => qa.SubmittedAt)
//                               .Select(qa => new GetStudentAttemptHistoryResponse
//                                      (
//                                          qa.Id,
//                                          qa.Quiz.Title,
//                                          qa.Result.Score,
//                                          qa.Status,
//                                          qa.SubmittedAt
//                                      )
//                               );
//        var result = await query.PaginateAsync(request.PageNumber , request.PageSize ,cancellationToken);
//        _logger.LogInformation(
//               "Attempt history returned for Student {StudentId}. TotalCount: {TotalCount}, Page: {Page}/{TotalPages}",
//               request.StudentId, result.TotalCount, result.PageNumber, result.TotalPages);
//        return RequestResult<PaginationResult<GetStudentAttemptHistoryResponse>>.succeeded(result ,ResultCode.QuizAttemptHistoryRetrievedSuccessfully);
//    }
//}