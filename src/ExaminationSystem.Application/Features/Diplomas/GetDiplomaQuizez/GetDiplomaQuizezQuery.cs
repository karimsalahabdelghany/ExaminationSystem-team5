using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomaQuizez;

public record GetDiplomaQuizezQuery
(Guid dipolmaId, Guid studentId) : IRequest<RequestResult<GetDiplomaQuizezResponse>>;

//public class GetDiplomaQuizezQueryHandler(IRepository<Diploma> repository 
//    ,UserManager<User> userManager , RoleManager<IdentityRole<Guid>> roleManager) : IRequestHandler<GetDiplomaQuizezQuery, RequestResult<GetDiplomaQuizezResponse>>
//{
//    private readonly IRepository<Diploma> _repository = repository;
//    private readonly UserManager<User> _userManager = userManager;
//    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;

//    public async Task<RequestResult<GetDiplomaQuizezResponse>> Handle(GetDiplomaQuizezQuery request, CancellationToken cancellationToken)
//    {
//        var diploma = await _repository.GetAll(d => d.Id == request.dipolmaId
//                                              && d.Status == DiplomaStatus.Published
//                                              && d.Enrollments.Any(e => e.UserId == request.studentId))
//                                       .Select(d => new
//                                       {
//                                           diplomaId = d.Id,
//                                           Quizez = d.Quizzes.Select(q =>new
//                                           {
//                                               q.Id,
//                                               q.Status,
//                                               q.Title,
//                                               q.DurationMinutes
//                                           }).ToList(),
                                          
//                                       }).FirstOrDefaultAsync(cancellationToken);
//        if(diploma == null)
//            return RequestResult<GetDiplomaQuizezResponse>.Failure(null, ResultCode.DiplomaNotFound);
//        var userQuizezInfo = await _userManager.Users.Where(u => u.Id == request.studentId)
//            .Select(u => new
//            {
//                QuizezAttempts = u.QuizAttempts.Where(qa => diploma.Quizez.Select(d => d.Id).Contains(qa.QuizId))
//                                               .Select(qa => new
//                                               {
//                                                   qa.QuizId,
//                                                   qa.Result.Score,
//                                               }).ToList()


//            }).FirstOrDefaultAsync(cancellationToken);
//        if (diploma is null)
//            return RequestResult<GetDiplomaQuizezResponse>.Failure(null, ResultCode.DiplomaNotFound);
//        return RequestResult<GetDiplomaQuizezResponse>.succeeded(new GetDiplomaQuizezResponse
//        {
            
//        });
//    }
//}
