namespace ExaminationSystem.Application.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string entityName, string reason)
        : base($"Conflict on '{entityName}': {reason}")
    {
    }
}
