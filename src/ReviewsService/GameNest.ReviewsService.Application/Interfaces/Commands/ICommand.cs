using MediatR;

namespace GameNest.ReviewsService.Application.Interfaces.Commands
{
    public interface ICommand : IRequest<Unit> { }
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
