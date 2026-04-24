using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Diplomas.CheckDiplomaExist;
using ExaminationSystem.Application.Responses;
using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomasQuizzes
{
    public record GetDiplomaQuizzesQuery(
    Guid DiplomaId,
    Guid StudentId,
    PaginationParams Pagination
) : IRequest<RequestResult<PaginatedResult<GetDiplomaQuizzesResponse>>>;
   
    public class GetDiplomaQuizzesQueryHandler
        : IRequestHandler<GetDiplomaQuizzesQuery, RequestResult<PaginatedResult<GetDiplomaQuizzesResponse>>>
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public GetDiplomaQuizzesQueryHandler(
            IUnitOfWork unitOfWork
             ,IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<RequestResult<PaginatedResult<GetDiplomaQuizzesResponse>>> Handle(
            GetDiplomaQuizzesQuery request,
            CancellationToken cancellationToken)
        {
            var QuizRepo = _unitOfWork.Repository<Quiz>();
            // Validate diploma exists and is published
            // requestResult<bool>
            var diplomaExists = await _mediator.Send(new CheckDiplomaExistQuery(request.DiplomaId));


            if (!diplomaExists.Success)
                return RequestResult<PaginatedResult<GetDiplomaQuizzesResponse>>
                 .Failure(null , ResultCode.DiplomaNotFound);



            var result = await QuizRepo
                .GetAll()
                .AsNoTracking()
                .Where(q => q.DiplomaId == request.DiplomaId
                         && q.Status == QuizStatus.Published
                         && !q.IsDeleted)
                .OrderByDescending(q => q.CreatedAt)

                .Select(q => new GetDiplomaQuizzesResponse(
                    Id: q.Id,
                    Title: q.Title,
                    DurationMinutes: q.DurationMinutes,
                    AttemptCount: q.QuizAttempts.Count(a => a.UserId == request.StudentId),
                    LastScore: q.QuizAttempts
                        .Where(a => a.UserId == request.StudentId
                                 && a.Result != null)
                        .OrderByDescending(a => a.StartTime)
                        .Select(a => (decimal?)a.Result!.Score)
                        .FirstOrDefault(),
                    Status: q.Status
                ))
                
                .ToPagedAsync(request.Pagination, cancellationToken);

            return RequestResult<PaginatedResult<GetDiplomaQuizzesResponse>>.succeeded(result,ResultCode.RecentQuizAttemptsloadedSuccessfuly);
        }
    }
}
