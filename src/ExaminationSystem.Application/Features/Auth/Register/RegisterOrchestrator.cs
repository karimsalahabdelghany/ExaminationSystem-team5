using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.OTP.GenerateNewOtp;
using ExaminationSystem.Application.Services.EmailService;
using ExaminationSystem.Domain.Enums;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace ExaminationSystem.Application.Features.Auth.Register;

    public record RegisterOrchestrator(string FullName, string Email, string Password) : IRequest<RequestResult<RegisterResponse>>;

    public class RegisterOrchestratorHandler : IRequestHandler<RegisterOrchestrator, RequestResult<RegisterResponse>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public RegisterOrchestratorHandler(UserManager<AppUser> userManager 
            ,IMediator mediator
            ,IUnitOfWork unitOfWork ,IEmailService emailService
            ,  IPasswordHasher<AppUser> passwordHasher)
        {
            _userManager = userManager;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _emailService = emailService; 
            _passwordHasher = passwordHasher;   
        }

        public   async Task<RequestResult<RegisterResponse>> Handle(RegisterOrchestrator request, CancellationToken cancellationToken)
        {
            var email= _userManager.NormalizeEmail(request.Email);  
                var isExistEmail = await _userManager.Users
                                        .AnyAsync(u => u.NormalizedEmail == email , cancellationToken);
            
            if (isExistEmail)
            {
                 return RequestResult<RegisterResponse>.Failure(null , ResultCode.UserIsAlredyExist); 
            }

            var newUser = request.Adapt<AppUser>();
            newUser.Id = Guid.CreateVersion7();
            newUser.Status = AccountStatus.PendingVerification;
            newUser.UserName = email;
            await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            var createUser = await _userManager.CreateAsync(newUser , request.Password);
            if(!createUser.Succeeded)
            {
                var errors = createUser.Errors.Select(e => e.Description).ToList();
                await _unitOfWork.RollbackAsync(cancellationToken);
                return RequestResult<RegisterResponse>
                    .Failure(new RegisterResponse(newUser.Id, errors), ResultCode.UserCreateFilad);
            }
            var otp = await _mediator.Send(new GenerateNewOtpCommand(newUser.Id ,OtpPurpose.EmailConfirmation), cancellationToken);
            if(!otp.Success)
            {
               await _unitOfWork.RollbackAsync(cancellationToken);
               return RequestResult<RegisterResponse>
                   .Failure(new RegisterResponse(newUser.Id, new List<string> { "Failed to generate OTP" }), ResultCode.UserCreateFilad);
            }
            var sendEmailResult  = await _emailService.SendAsync(new EmailRequest
            {
                To = newUser.Email!,
                Subject = "Your OTP Code",
                Body = $"Your OTP is: {otp.Result.otpCode}",
                IsHtml = false
            }, cancellationToken);

            if(!sendEmailResult.IsSuccess)
            {
               await _unitOfWork.RollbackAsync(cancellationToken);
               
               return  RequestResult<RegisterResponse>
                   .Failure(new RegisterResponse(newUser.Id, new List<string> { sendEmailResult.ErrorMessage }), ResultCode.FailedToSendRegisterEmail);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            return RequestResult<RegisterResponse>.succeeded(
                new RegisterResponse(newUser.Id),
                ResultCode.UserCreateSuccesfully
            );
        }
    }


