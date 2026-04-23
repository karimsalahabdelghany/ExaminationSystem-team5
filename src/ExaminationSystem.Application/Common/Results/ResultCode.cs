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

    // Attempts (1200-1299)
    AttemptNotFound = 1200,
    AttemptNotOwned = 1201,
    AttemptAlreadySubmitted = 1202,
    AttemptTimedOut = 1203,
    QuestionNotInQuiz = 1204,
    AnswerSavedSuccessfully = 1205,
    SubmitAttemptSuccessful = 1206,

    // Start quiz attempt (POST .../quizzes/{id}/start)
    QuizNotFoundOrNotPublished = 2000,
    QuizAttemptStartedSuccessfully = 2001,
    AttemptAlreadyInProgress = 2002,
    AttemptLimitReached = 2003,
    AttemptStartConflict = 2004,
}
