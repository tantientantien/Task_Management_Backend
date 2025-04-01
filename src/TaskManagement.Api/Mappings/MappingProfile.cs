using AutoMapper;

namespace TaskManagement.Api.Mappings;
class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDataDto>();
        CreateMap<CategoryWriteDto, Category>();


        CreateMap<Label, LabelDataDto>();
        CreateMap<LabelWriteDto, Label>();

        CreateMap<TaskLabel, LabelDataDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LabelId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Label.Name))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Label.Color));


        CreateMap<TaskItem, TaskDataDto>()
            .ForMember(dest => dest.AttachmentCount, opt => opt.MapFrom(src => src.Attachments.Count))
            .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.TaskComments.Count));
        CreateMap<TaskWriteDto, TaskItem>();
        CreateMap<TaskPatchDto, TaskItem>()
            .ForMember(dest => dest.Title, opt => opt.Condition(src => src.Title != null))
            .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
            .ForMember(dest => dest.IsCompleted, opt => opt.Condition(src => src.IsCompleted.HasValue))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom((src, dest) => src.CategoryId ?? dest.CategoryId))
            .ForMember(dest => dest.AssigneeId, opt => opt.MapFrom((src, dest) => !string.IsNullOrEmpty(src.AssigneeId) ? src.AssigneeId : dest.AssigneeId))
            .ForMember(dest => dest.Duedate, opt => opt.MapFrom((src, dest) => src.Duedate ?? dest.Duedate));



        CreateMap<TaskComment, CommentDataDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => new UserDataDto { Id = src.UserId }));


        CreateMap<CommentWriteDto, TaskComment>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<TaskAttachment, AttachmentDataDto>().ReverseMap();
    }
}