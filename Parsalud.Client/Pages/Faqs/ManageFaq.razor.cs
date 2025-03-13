using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Faqs;

public partial class ManageFaq : BaseAbm<FaqModel, IFaqService, ParsaludFaq, ManageFaqRequest, FaqSearchCriteria>
{
    public ManageFaq() : base("Dashboard/Faqs")
    {
    }

    public ParsaludService[] Services { get; set; } = [];

    protected async override Task OnInitializedAsync()
    {
        var response = await ServiceService.GetByCriteriaAsync(new ServiceSearchCriteria());

        if (response.IsSuccessWithData)
        {
            Services = response.Data;
        }

        await base.OnInitializedAsync();
    }
}