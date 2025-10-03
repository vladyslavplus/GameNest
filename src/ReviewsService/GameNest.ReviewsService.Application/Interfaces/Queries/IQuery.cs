using MediatR;

namespace GameNest.ReviewsService.Application.Interfaces.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
