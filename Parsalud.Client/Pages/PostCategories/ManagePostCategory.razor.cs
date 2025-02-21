using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.PostCategories;

public partial class ManagePostCategory : BaseAbm<PostCategoryModel, IPostCategoryService, ParsaludPostCategory, ManagePostCategoryRequest, PostCategorySearchCriteria>
{
    public ManagePostCategory() : base("Dashboard/PostCategories")
    {    
    }
}