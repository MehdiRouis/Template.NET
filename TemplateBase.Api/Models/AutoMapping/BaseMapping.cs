using AutoMapper;

using TemplateBase.Api.Models.Globals;
using TemplateBase.Core;

namespace TemplateBase.Api.Models.AutoMapping
{
    public class BaseMapping : Profile
    {
        public BaseMapping()
        {
            CreateMap<BaseEntity, BaseModel>();
        }
    }
}
