using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.PostCategories;

public class PostCategoryModel : IModelBase<PostCategoryModel, ManagePostCategoryRequest, ParsaludPostCategory>
{
    public PostCategoryModel()
    {
    }

    public Guid? Id { get; set; }
    public string? Name { get; set; }

    public PostCategoryModel FromDto(ParsaludPostCategory dto)
    {
        return new PostCategoryModel
        {
            Id = dto.Id,
            Name = dto.Name,
        };
    }

    public ManagePostCategoryRequest ToRequest()
    {
        return new ManagePostCategoryRequest
        {
            Name = Name ?? "",
        };
    }
}
