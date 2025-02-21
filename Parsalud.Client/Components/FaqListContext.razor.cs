using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Components;

public partial class FaqListContext : ParsaludComponent
{
    [Parameter]
    public string? View { get; set; }

    [Parameter]
    public string? LoadingView { get; set; }

    [Parameter]
    public string? Category { get; set; }

    [Parameter]
    public string? MaxCount { get; set; }

    private ParsaludFaq[]? items;

    protected override async Task OnInitializedAsync()
    {
        _ = int.TryParse(MaxCount, out int maxCountInt);

        var response = await FaqService.GetByCriteriaAsync(new FaqSearchCriteria
        {
            Category = Category,
            MaxCount = maxCountInt == 0 ? null : maxCountInt,
        });

        if (response.IsSuccessWithData)
        {
            items = response.Data;
        }
        else
        {
            items = [];
        }
    }


    protected override string GetComponentCssClass()
    {
        return "accordion";
    }
}
