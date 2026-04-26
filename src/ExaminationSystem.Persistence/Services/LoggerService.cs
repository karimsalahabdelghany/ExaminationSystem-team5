using ExaminationSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ExaminationSystem.Persistence.Services;

public class LoggerService(ILogger<LoggerService> logger) : ILoggerService
{
    public void LogInformation(string messageTemplate, params object[] propertyValues)
    {
        logger.LogInformation(messageTemplate, propertyValues);
    }

    public void LogWarning(string messageTemplate, params object[] propertyValues)
    {
        logger.LogWarning(messageTemplate, propertyValues);
    }

    public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
    {
        logger.LogError(exception, messageTemplate, propertyValues);
    }
}
