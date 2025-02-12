using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.PostCategories;

public class PostCategoryModel
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }

    public ManagePostCategoryRequest ToRequest()
    {
        return new ManagePostCategoryRequest
        {
            Name = Name ?? "",
        };
    }
}
