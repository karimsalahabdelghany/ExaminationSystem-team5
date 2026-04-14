using ExaminationSystem.Application.Common.Helper;
using ExaminationSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;


namespace ExaminationSystem.Application.Common.Behaviours;

// Application/Common/Behaviours/UnitOfWorkBehavior.cs
public class UnitOfWorkBehavior<TRequest, TResponse>(
    IUnitOfWork session,
    TransactionScope scope,
    ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(
     TRequest request,
     RequestHandlerDelegate<TResponse> next,
     CancellationToken ct)
    {
        using var handle = scope.Begin();
        var isRoot = scope.IsRoot; // ✅ captured — safe against any future parallel use

        if (!isRoot)
            return await next();

        await session.BeginTransactionAsync(ct);

        try
        {
            var response = await next();
            await session.CommitAsync(ct);
            return response;
        }
        catch (Exception ex)
        {
            await session.RollbackAsync(ct);
            logger.LogError(ex, "[ROOT] {Request} rolled back", typeof(TRequest).Name);
            throw;
        }
    }
}


//public class UnitOfWorkBehavior<TRequest, TResponse>(
//    IUnitOfWork unitOfWork,
//    TransactionScope scope, ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
//    : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : ICommand<TResponse>
//{
//    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger = logger;

//    public async Task<TResponse> Handle(
//        TRequest request,
//        RequestHandlerDelegate<TResponse> next,
//        CancellationToken ct)
//    {
//        _logger.LogInformation("Starting transaction for {RequestName}", typeof(TRequest).Name);
//        using (scope.Begin())
//        {
//            var response = await next();
//            if (scope.IsRoot)
//            {
//                try
//                {
//                    var result = await unitOfWork.CommitAsync(ct);
//                    _logger.LogInformation($"Number of effective rows : {result}");
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError($"Error Ocure when saving changes in entity due to : {ex}");
//                    throw ex;
//                }
//            }
//            return response;
//        }
//    }
//}

