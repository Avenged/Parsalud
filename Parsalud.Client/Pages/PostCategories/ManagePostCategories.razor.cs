using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.PostCategories;

public partial class ManagePostCategories : ManagerBase<IPostCategoryService, ParsaludPostCategory, ManagePostCategoryRequest, PostCategorySearchCriteria>
{
    public ManagePostCategories() : base("Dashboard/PostCategory/Create", "Nueva categoría")
    {  
    }
}
