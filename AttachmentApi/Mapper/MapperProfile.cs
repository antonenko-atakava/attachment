using AttachmentApi.Database.DTO;
using AttachmentApi.Database.Models;
using AutoMapper;

namespace AttachmentApi.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Attachment, AttachmentDto>();
        CreateMap<AttachmentDto, Attachment>();
    }
}