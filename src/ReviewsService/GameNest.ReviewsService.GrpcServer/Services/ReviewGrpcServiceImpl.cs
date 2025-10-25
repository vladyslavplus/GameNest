using AutoMapper;
using GameNest.Grpc.Reviews;
using GameNest.ReviewsService.Domain.Entities.Parameters;
using GameNest.ReviewsService.Domain.Exceptions;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GameNest.ReviewsService.Grpc.Services
{
    public class ReviewGrpcServiceImpl : ReviewGrpcService.ReviewGrpcServiceBase
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewGrpcServiceImpl> _logger;

        public ReviewGrpcServiceImpl(
            IReviewService reviewService,
            IMapper mapper,
            ILogger<ReviewGrpcServiceImpl> logger)
        {
            _reviewService = reviewService;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<PagedReviewsResponse> GetReviews(
            GetReviewsRequest request,
            ServerCallContext context)
        {
            try
            {
                var parameters = new ReviewParameters
                {
                    PageNumber = request.PageNumber > 0 ? request.PageNumber : 1,
                    PageSize = request.PageSize > 0 ? request.PageSize : 10,
                    OrderBy = string.IsNullOrWhiteSpace(request.OrderBy) ? "Id" : request.OrderBy,
                    GameId = string.IsNullOrWhiteSpace(request.GameId) ? null : request.GameId,
                    CustomerId = string.IsNullOrWhiteSpace(request.CustomerId) ? null : request.CustomerId,
                    SearchText = string.IsNullOrWhiteSpace(request.SearchText) ? null : request.SearchText
                };

                var pagedReviews = await _reviewService.GetReviewsAsync(
                    parameters,
                    context.CancellationToken);

                var response = new PagedReviewsResponse
                {
                    TotalCount = pagedReviews.TotalCount,
                    PageNumber = pagedReviews.CurrentPage,
                    PageSize = pagedReviews.PageSize
                };

                response.Items.AddRange(pagedReviews.Select(r => _mapper.Map<Review>(r)));

                _logger.LogInformation(
                    "Returned {Count} reviews via gRPC (GameId: {GameId}, CustomerId: {CustomerId})",
                    response.Items.Count,
                    parameters.GameId ?? "all",
                    parameters.CustomerId ?? "all");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetReviews gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<ReviewResponse> GetReviewById(
            GetReviewByIdRequest request,
            ServerCallContext context)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    throw new RpcException(
                        new Status(StatusCode.InvalidArgument, "Review ID is required"));
                }

                var review = await _reviewService.GetReviewByIdAsync(
                    request.Id,
                    context.CancellationToken);

                if (review == null)
                {
                    throw new RpcException(
                        new Status(StatusCode.NotFound, $"Review with ID {request.Id} not found"));
                }

                var response = new ReviewResponse
                {
                    Review = _mapper.Map<Review>(review)
                };

                _logger.LogInformation("Returned review {ReviewId} via gRPC", request.Id);

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Review not found: {ReviewId}", request.Id);
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetReviewById gRPC call");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}