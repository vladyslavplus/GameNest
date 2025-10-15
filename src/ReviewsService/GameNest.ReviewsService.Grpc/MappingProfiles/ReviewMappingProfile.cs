using AutoMapper;
using GameNest.Grpc.Reviews;
using DomainReview = GameNest.ReviewsService.Domain.Entities.Review;
using DomainReply = GameNest.ReviewsService.Domain.Entities.Reply;

namespace GameNest.ReviewsService.Grpc.MappingProfiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<DomainReview, Review>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text.Value))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating.Value))
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies));

            CreateMap<DomainReply, Reply>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text.Value));
        }
    }
}
