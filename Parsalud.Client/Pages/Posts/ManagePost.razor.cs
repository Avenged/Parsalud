using Parsalud.Client.Components;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Posts;

public partial class ManagePost : BaseAbm<PostModel, IPostService, ParsaludPost, ManagePostRequest, PostSearchCriteria>
{
    public ManagePost() : base("Dashboard/Posts")
    {    
    }

    private int i;

    public ParsaludPostCategory[] Categories { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var response = await PostCategoryService.GetByCriteriaAsync(new PostCategorySearchCriteria());

        if (response.IsSuccessWithData)
        {
            Categories = response.Data;
        }

        await base.OnInitializedAsync();
    }

    private void CategoryChanged()
    {
        var cat = Categories.FirstOrDefault(x => x.Id == Model.PostCategoryId);
        Model.PostCategory = cat?.Name;
        Increment();
    }

    private void Increment()
    {
        i++;
    }
}