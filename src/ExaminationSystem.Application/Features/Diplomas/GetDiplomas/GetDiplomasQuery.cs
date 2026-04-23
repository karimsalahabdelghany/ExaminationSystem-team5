using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Responses;
using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomas
{
    public record GetDiplomasQuery(
    PaginationParams Params
    ) : IRequest<RequestResult<PaginatedResult<GetDiplomasResponse>>>;

    public class GetDiplomasQueryHandler
    : IRequestHandler<GetDiplomasQuery, RequestResult<PaginatedResult<GetDiplomasResponse>>>
    {
        private readonly IRepository<Diploma> _diplomaRepo;
        private readonly ICurrentUser _currentUser;

        public GetDiplomasQueryHandler(IRepository<Diploma> diplomaRepo ,ICurrentUser currentUser)
        {
            _diplomaRepo = diplomaRepo;
            _currentUser = currentUser;
        }

        public async Task<RequestResult<PaginatedResult<GetDiplomasResponse>>> Handle(GetDiplomasQuery request, CancellationToken cancellationToken)
        {
            var result = await _diplomaRepo
           .GetAll()
           .AsNoTracking()
           .Where(d => d.Status == DiplomaStatus.Published && !d.IsDeleted)
           .OrderByDescending(d => d.CreatedAt)
           .Select(d => new GetDiplomasResponse(
               Id: d.Id,
               Title: d.Title,
               Description: d.Description,
               QuizCount: d.Quizzes.Count(q => q.Status == QuizStatus.Published
                                                  && q.IsDeleted == false),
               StudentProgress: d.Quizzes.Count(q => q.Status == QuizStatus.Published
                                                   && !q.IsDeleted) == 0
                   ? 0m
                   : Math.Round(
                       (decimal)d.Quizzes.Count(q =>
                           q.Status == QuizStatus.Published
                        && q.DeletedAt == null
                        && q.QuizAttempts.Any(a =>
                               a.UserId == _currentUser.Id
                            && a.Status != QuizAttemptStatus.InProgress))
                       / d.Quizzes.Count(q => q.Status == QuizStatus.Published
                                           && !q.IsDeleted) * 100, 2)
           )).ToPagedAsync(request.Params, cancellationToken);

            return RequestResult<PaginatedResult<GetDiplomasResponse>>.succeeded(result,ResultCode.DiplomasRetrievedSuccessfully);
        }
    }
}
