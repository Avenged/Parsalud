using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;
using System.Collections;

namespace Parsalud.Client.Pages.Sections;

public partial class ManageSections : ComponentBase
{
    public const string NEW_ITEM_TEXT = "Nueva sección";
    public const string CREATE_PATH = "Dashboard/Section/Create";

    public SectionSearchCriteria Criteria { get; } = new();
    public ParsaludSection[]? Items { get; set; }
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
