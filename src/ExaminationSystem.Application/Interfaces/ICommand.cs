using MediatR;

namespace ExaminationSystem.Application.Interfaces;

public interface ICommand : IRequest
{
}
public interface ICommand<TResponse> : IRequest<TResponse>
{
}
