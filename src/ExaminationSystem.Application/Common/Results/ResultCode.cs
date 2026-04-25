namespace ExaminationSystem.Application.Common.Results;

public enum ResultCode
{   //Admin 100-200
    AdminStatsDataAlreadyCachedinMemory = 100,
    AdminStatsDataNotFound = 101,
    AdminStatsQueryFiredSuccessfully = 102,
    AvgPassRateFailed = 103,
    AvgPassRateSuccessed = 104,
    UsersLoginTodaySuccessfully = 105,
    GetTotalUsersQuerySuccessed = 106,
    AdminAttemptDetailsRetrievedSuccessfully = 107,


    //User  200-300
    UserHasDiplomaEnrollments = 201,
    RecentQuizAttemptsloadedSuccessfuly = 202,
    OverAllStatsProgressQuerySucessful = 203,
    StudentsDashoardQuerySucessfull = 204,
    StudentsDashoardQueryFalied = 205,
    StudentStatsDataAlreadyCachedinMemory = 206,
    TotalAttemptsQuerySuccessfull = 207,
    TotalQuzizesQuerySucessfull = 208,


    //Diploma
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

    // Quizzes (1100-1199)
    QuizNotFound = 1100,
    QuizCreatedSuccessfully = 1101,
    QuizUpdatedSuccessfully = 1102,
    QuizDeletedSuccessfully = 1103,
    QuizIsPublished = 1104,
    QuizHasActiveAttempts = 1105,
    QuizAlreadyPublished = 1106,
    QuizPublishedSuccessfully = 1107,
    QuizHasNoQuestions = 1108,
    QuizAlreadyDraft = 1109,
    QuizUnpublishedSuccessfully = 1110,
    QuizIsExist = 1111,


    // Attempts (1200-1299)
    AttemptNotFound = 1200,
    AttemptNotOwned = 1201,
    AttemptAlreadySubmitted = 1202,
    AttemptTimedOut = 1203,
    QuestionNotInQuiz = 1204,
    AnswerSavedSuccessfully = 1205,
    SubmitAttemptSuccessful = 1206,
    QuizAttemptHistoryRetrievedSuccessfully = 1207,
    AttemptDetailsRetrievedSuccessfully = 1208,
    AttemptResultsNotAvailableYet = 1209,
    AttemptResultsRetrievedSuccessfully = 1210,
    AttemptResultsNotFound = 1211,

    // Start quiz attempt (POST .../quizzes/{id}/start)
    QuizNotFoundOrNotPublished = 2000,
    QuizAttemptStartedSuccessfully = 2001,
    AttemptAlreadyInProgress = 2002,
    AttemptLimitReached = 2003,
    AttemptStartConflict = 2004,

    // Questions (1300-1399)
    FailedToCreate = 1300,
    QuestionCreatedSuccessfully = 1301,
    QuestionNotFound = 1302,
    QuestionIsExist = 1303,
    QuestionUpdatedSuccessfully = 1304,
    QuestionDeletedSuccessfully = 1305,
    QuestionDeleteFailed = 1306,
    QuestionFailedToUpdate = 1307,
    QuestionNotFoundOrQuizPublished = 1308,
    QuestionIsExistAndQuizPublished = 1309,
    QuestionHasMoreThanOneCorrectAnswer = 1310,


    //Options (1400-1499)
    OptionsNotFound = 1400,
    OptionIsExist = 1401,
    OptionCreatedSuccessfully = 1402,
    OptionUpdatedSuccessfully = 1403,
    OptionDeletedSuccessfully = 1404,
    OptionDeleteFailed = 1405,
    OptionFailedToUpdate = 1406,
    QuestionBasicInfoUpdatedSuccessfully = 1407,


    // Register Code 
    UserIsAlredyExist = 2000 , 
    UserCreateSuccesfully = 2001 , 
    UserCreateFilad = 2002 , 
    FailedToSendRegisterEmail=2003,
    CanNotVerifyAccount = 2004,
    // OTP 
    UserIsNotExsit = 20003,
    UserEmailIsNotExistOrAccountIsNotInPendingStatus = 2010,
    ResendLimitExceeded = 2004 , 
    OTPResentSuccessfully = 2005,
    AccountActivatedSuccessfully =2006 ,
    NoActiveOTPFound =2007 ,
    AccountLocked = 2008 , 
    OtpExpried = 2009  ,
    OtpNotVaild = 3001 ,
    OtpGeneratedSuccessfully = 3002,
    OtpVerified = 3003,

    // Login 
    Invalidcredentials = 401  ,
    AccountNotverified = 402,

    // New Auth Codes (5000-5099)
    LoginSucceeded = 5000,
    InvalidCredentials = 5001,
    AccountNotActive = 5002,
    AccountLockedTemporarily = 5003,

    TokenRefreshedSuccessfully = 5010,
    RefreshTokenInvalid = 5011,
    RefreshTokenExpired = 5012,
    RefreshTokenRevoked = 5013


}
