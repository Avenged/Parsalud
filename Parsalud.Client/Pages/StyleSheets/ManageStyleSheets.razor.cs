using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.StyleSheets;

public partial class ManageStyleSheets : ComponentBase
{
    public const string NEW_ITEM_TEXT = "Nueva hoja de estilo";
    public const string CREATE_PATH = "Dashboard/StyleSheet/Create";

    public StyleSheetSearchCriteria Criteria { get; } = new();
    public ParsaludStyleSheet[]? Items { get; set; }
    public bool IsSearching { get; set; }

    public async Task Submit()
    {
        Items = null;
        IsSearching = true;
        await Task.Yield();

        var response = await Service.GetByCriteriaAsync(Criteria);

        if (response.IsSuccess)
        {
            Items = response.Data;
        }

        IsSearching = false;
    }

    public void CreateNew()
    {
        NM.NavigateTo(CREATE_PATH);
    }
}
