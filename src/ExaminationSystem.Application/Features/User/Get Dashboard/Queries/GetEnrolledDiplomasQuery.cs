using ExaminationSystem.Application.Features.Enrollments.Queries;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;                    
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Users.Get_Dashboard.Queries
{
    public record GetEnrolledDiplomasQuery(Guid StudentId) : IQuery<RequestResult<IEnumerable<EnrolledDiplomasResponse>>>
    {
    }
   
    public class GetEnrolledDiplomasQueryHandler
        : IRequestHandler<GetEnrolledDiplomasQuery,RequestResult<IEnumerable<EnrolledDiplomasResponse>>>
    {
        private readonly IRepository<Enrollment> _enrollmentRepo;

        public GetEnrolledDiplomasQueryHandler(IRepository<Enrollment> enrollmentRepo)
            => _enrollmentRepo = enrollmentRepo;


        public async Task<RequestResult<IEnumerable<EnrolledDiplomasResponse>>> Handle(
        GetEnrolledDiplomasQuery request,
        CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepo
                .GetAll(e => e.UserId == request.StudentId && e.Diploma.Status == DiplomaStatus.Published)
                .Select(e => new
                {
                    e.DiplomaId,
                    e.Diploma.Title,
                    e.Diploma.Description,
                    Quizzes = e.Diploma.Quizzes.Where(q => q.Status == QuizStatus.Published)
                })
                .ToListAsync(cancellationToken);

            var result = enrollments.Select(e => {
                var totalQuizzes = e.Quizzes.Count();

                var completedQuizzes = e.Quizzes.Count(q => q.QuizAttempts
                                                .Any(a => a.UserId == request.StudentId
                                                       && a.Status != QuizAttemptStatus.InProgress));

                return new EnrolledDiplomasResponse(
                    DiplomaId: e.DiplomaId,
                    Title: e.Title,
                    Description: e.Description ?? string.Empty,
                    TotalQuizzes: totalQuizzes,
                    CompletedQuizzes: completedQuizzes,
                    ProgressPercent: totalQuizzes == 0 ? 0m : Math.Round((decimal)completedQuizzes / totalQuizzes * 100, 2)
                );
            });

            return RequestResult<IEnumerable<EnrolledDiplomasResponse>>.succeeded(result, ResultCode.UserHasDiplomaEnrollments);
        }
    }

        
    
}
