using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Posts;

public partial class ManagePosts : ManagerBase<IPostService, ParsaludPost, PostSearchCriteria>
{
    public const string NEW_ITEM_TEXT = "Nuevo artículo";
    public const string CREATE_PATH = "Dashboard/Post/Create";

    public void CreateNew()
    {
        NM.NavigateTo(CREATE_PATH);
    }
}
