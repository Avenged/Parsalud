using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.PostCategories;

public partial class ManagePostCategories : ManagerBase<IPostCategoryService, ParsaludPostCategory, PostCategorySearchCriteria>
{
    public const string NEW_ITEM_TEXT = "Nueva categoría";
    public const string CREATE_PATH = "Dashboard/PostCategory/Create";

    public void CreateNew()
    {
        NM.NavigateTo(CREATE_PATH);
    }
}
