namespace ExaminationSystem.Application.Common.Results;

public enum ResultCode
{
    WeakPassword = 201 ,
    ValidationError = 202 ,
    DiplomaNotFound = 1000,
    DiplomaHasEnrollments = 1001,
    DiplomaDeletedSuccessfully = 1002,
    DiplomaCreatedSuccessfully = 1003,
    DiplomaUpdatedSuccessfully = 1004,
    DiplomaIsPublished = 1005,
    DiplomaHasActiveEnrollments = 1006,
    DiplomaHasActiveEnrollmentsOrPublished = 1007,
    DiplomaExist = 1008,
    DiplomasRetrievedSuccessfully = 1009,
    StudentNotEnrolledInDiploma = 1010,
    StudentEnrolledInDiploma = 1011,
    // Register Code 
    UserIsAlredyExist = 2000 , 
    UserCreateSuccesfully = 2001 , 
    UserCreateFilad = 2002 , 
    // OTP 
    UserIsNotExsit = 20003,
    ResendLimitExceeded = 2004 , 
    OTPResentSuccessfully = 2005,
    AccountActivatedSuccessfully =2006 ,
    NoActiveOTPFound =2007 ,
    AccountLocked = 2008 , 
    OtpExpried = 2009  ,
    OtpNotVaild = 3001 , 

    // Login 
    Invalidcredentials = 401  ,
    AccountNotverified = 402








}
