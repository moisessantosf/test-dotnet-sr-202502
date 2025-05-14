using ApplicantTracking.Application.DTOs;
using ApplicantTracking.Domain.Entities;
using AutoMapper;

namespace ApplicantTracking.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Candidate, CandidateDto>().ReverseMap();
            CreateMap<CreateCandidateDto, Candidate>();
            CreateMap<UpdateCandidateDto, Candidate>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
