using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Posts;

public partial class ManagePost : BaseAbm<Guid>
{
    public PostModel Model { get; set; } = new();

    public void Discard()
    {
        NM.NavigateTo("Dashboard/Posts");
    }
}