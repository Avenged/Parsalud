using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IPostCategoryService : IManagerServiceBase<ParsaludPostCategory, ManagePostCategoryRequest, PostCategorySearchCriteria>
{
}