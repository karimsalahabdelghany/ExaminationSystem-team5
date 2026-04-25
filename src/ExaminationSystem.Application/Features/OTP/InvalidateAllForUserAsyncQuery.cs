using ExaminationSystem.Application.Common.Results;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.OTP
{
    public record InvalidateAllForUserAsyncQuery(Guid userId) : IQuery<GetActiveOptByUserResponse>; 
       public record GetActiveOptByUserResponse(string otpCode);
    public class InvalidateAllForUserAsyncQueryHandler : IRequestHandler<InvalidateAllForUserAsyncQuery, GetActiveOptByUserResponse>
    {
        private readonly IRepository<OtpCode> _repository;

        public InvalidateAllForUserAsyncQueryHandler(IRepository<OtpCode> repository)
        {
           _repository = repository;
        }
        public async Task<GetActiveOptByUserResponse> Handle(InvalidateAllForUserAsyncQuery request, CancellationToken ct)
        {
         var res = await _repository.GetAll(x => x.UserId == request.userId && !x.IsUsed)
                .OrderByDescending(x => x.ExpiresAt)
                .FirstOrDefaultAsync(ct);


            return null; /*RequestResult<GetActiveOptByUserResponse>.succeeded(res.Adapt<GetActiveOptByUserResponse>(), ResultCode.AccountActivatedSuccessfully); */

        }
    }
}
