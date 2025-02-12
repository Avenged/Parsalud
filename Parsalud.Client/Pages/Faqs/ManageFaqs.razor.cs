using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Faqs;

public partial class ManageFaqs : ComponentBase
{
    public const string NEW_ITEM_TEXT = "Nuevo FAQ";
    public const string CREATE_PATH = "Dashboard/Faq/Create";

    public FaqSearchCriteria Criteria { get; } = new();
    public ParsaludFaq[]? Items { get; set; }
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
