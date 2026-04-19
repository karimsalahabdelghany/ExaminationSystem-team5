namespace ExaminationSystem.Application.Common.Results;

public enum ResultCode
{   //Admin
    AdminStatsDataAlreadyCashedinMemory = 1,
    AdminStatsDataNotFound = 2,
    AdminStatsQueryFiredSuccessfully = 3,
    AvgPassRateFailed = 4,
    AvgPassRateSuccessed = 5,
    UserHasDiplomaEnrollments = 6,
    RecentQuizAttemptsloadedSuccessfuly = 7,
    OverAllStatsProgressQuerySucessful = 8,
    UsersLoginTodaySuccessfully = 9,
    DiplomaNotFound = 1000,
    DiplomaHasEnrollments = 1001,
    DiplomaDeletedSuccessfully = 1002,
    DiplomaCreatedSuccessfully = 1003,
    DiplomaUpdatedSuccessfully = 1004,
    DiplomaIsPublished = 1005,
    DiplomaHasActiveEnrollments = 1006,
    DiplomaHasActiveEnrollmentsOrPublished = 1007,
    DiplomaExist = 1008,
}
