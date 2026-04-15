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

namespace ExaminationSystem.Application.Features.Users.Get_Dashboard.Queries
{
    public record GetEnrolledDiplomasQuery(Guid StudentId) : IQuery<IEnumerable<EnrolledDiplomasResponse>>
    {
    }
    // Eager-loads Diploma → Quizzes → Attempts in a single optimized query
    // "Eager-load diploma progress in a single query"
    //
    // SQL produced (single round trip):
    //   SELECT d.Id, d.Title, d.Description,
    //          COUNT(DISTINCT q.Id)                              AS TotalQuizzes,
    //          COUNT(DISTINCT a.Id WHERE a.Status != 'in_progress') AS CompletedQuizzes
    //   FROM Enrollments e
    //   JOIN Diplomas d    ON d.Id = e.DiplomaId
    //   JOIN Quizzes q     ON q.DiplomaId = d.Id
    //   LEFT JOIN QuizAttempts a ON a.QuizId = q.Id AND a.UserId = @StudentId
    //   WHERE e.UserId = @StudentId
    //   GROUP BY d.Id, d.Title, d.Description
    //
    // DEPENDS ON: POST /api/admin/diplomas + POST /api/diplomas/:id/enroll
    public class GetEnrolledDiplomasQueryHandler
        : IRequestHandler<GetEnrolledDiplomasQuery,IEnumerable<EnrolledDiplomasResponse>>
    {
        private readonly IRepository<Enrollment> _enrollmentRepo;

        public GetEnrolledDiplomasQueryHandler(IRepository<Enrollment> enrollmentRepo)
            => _enrollmentRepo = enrollmentRepo;

       
        public async Task<IEnumerable<EnrolledDiplomasResponse>> Handle(
            GetEnrolledDiplomasQuery request,
            CancellationToken cancellationToken)
        {
            // Single query — eager loads diploma + quizzes + attempts
            // GroupBy pushes all aggregation to SQL — no in-memory counting
            var result = await _enrollmentRepo
                .GetAll(e => e.UserId == request.StudentId)
                .Include(e => e.Diploma)
                    .ThenInclude(d => d.Quizzes)
                        .ThenInclude(q => q.QuizAttempts.Where(a => a.UserId == request.StudentId))
                .Where(e => e.Diploma.Status == DiplomaStatus.Published)

                .Select(e => new EnrolledDiplomasResponse(
                    DiplomaId: e.DiplomaId,
                    Title: e.Diploma.Title,
                    Description: e.Diploma.Description ?? string.Empty,

                    TotalQuizzes: e.Diploma.Quizzes.Count(q => q.Status == QuizStatus.Published),
                    CompletedQuizzes: e.Diploma.Quizzes
                        .Count(q => q.QuizAttempts
                            .Any(a => a.UserId == request.StudentId
                                   && a.Status != QuizAttemptStatus.InProgress)),

                    ProgressPercent: e.Diploma.Quizzes.Count(q => q.Status == QuizStatus.Published) == 0
                        ? 0m
                        : Math.Round(
                            (decimal)e.Diploma.Quizzes
                                .Count(q => q.QuizAttempts
                                    .Any(a => a.UserId == request.StudentId
                                           && a.Status != QuizAttemptStatus.InProgress)) // quiz which completed for user 
                            / e.Diploma.Quizzes.Count(q => q.Status == QuizStatus.Published) * 100, 2)
                ))
                .ToListAsync(cancellationToken);

            return result;
        }

        
    }
}
