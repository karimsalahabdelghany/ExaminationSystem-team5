namespace ExaminationSystem.Application.Common.Results;

public enum ResultCode
{
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
    UserNotEnrolledInDiploma = 1010,
    UserEnrolledInDiploma = 1011,
}
