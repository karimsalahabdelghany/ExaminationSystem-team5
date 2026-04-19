namespace ExaminationSystem.Application.Common.Results;

public enum ResultCode
{   //Admin
    AdminStatsDataAlreadyCashedinMemory = 1,
    AdminStatsDataNotFound = 2,
    AdminStatsQueryFiredSuccessfully = 3,
    AvgPassRateFailed = 4,
    AvgPassRateSuccessed = 5,




    //User
    UserHasDiplomaEnrollments = 6,
    RecentQuizAttemptsloadedSuccessfuly = 7,
    OverAllStatsProgressQuerySucessful = 8,
    UsersLoginTodaySuccessfully = 9,


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

    // Start quiz attempt (POST .../quizzes/{id}/start)
    QuizNotFoundOrNotPublished = 2000,
    QuizAttemptStartedSuccessfully = 2001,
    AttemptAlreadyInProgress = 2002,
    AttemptLimitReached = 2003,
    AttemptStartConflict = 2004,
}
