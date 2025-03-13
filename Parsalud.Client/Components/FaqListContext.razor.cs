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
    public string? MaxCount { get; set; }

    [Parameter]
    public string? ServiceCode { get; set; }

    private ParsaludFaq[]? items;
    private bool OnlyGeneric;

    protected override async Task OnInitializedAsync()
    {
        _ = int.TryParse(MaxCount, out int maxCountInt);

        if (ServiceCode?.Equals("{ServiceCode}") ?? false)
        {
            ServiceCode = null;
            OnlyGeneric = true;
        }

        var response = await FaqService.GetByCriteriaAsync(new FaqSearchCriteria
        {
            ServiceCode = ServiceCode,
            OnlyGeneric = OnlyGeneric,
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
