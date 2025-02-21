using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Posts;

public partial class ManagePosts : ManagerBase<IPostService, ParsaludPost, ManagePostRequest, PostSearchCriteria>
{
    public ManagePosts() : base("Dashboard/Post/Create", "Nuevo artículo")
    {  
    }

    public ParsaludPostCategory[] Categories { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var response = await PostCategoryService.GetByCriteriaAsync(new PostCategorySearchCriteria());

        if (response.IsSuccessWithData)
        {
            Categories = response.Data;
        }
    }
}
