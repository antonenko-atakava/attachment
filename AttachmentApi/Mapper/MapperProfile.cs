using AttachmentApi.DTO;
using AutoMapper;
using Database.Database.Entity;

namespace AttachmentApi.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Attachment, AttachmentDto>();
        CreateMap<AttachmentDto, Attachment>();
    }
}