namespace ExaminationSystem.Application.Interfaces;

public interface IQuery : IRequest
{
}
public interface IQuery<TResponse> : IRequest<TResponse>
{
}

