using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Faqs;

public partial class ManageFaq : BaseAbm<FaqModel, IFaqService, ParsaludFaq, ManageFaqRequest, FaqSearchCriteria>
{
    public ManageFaq() : base("Dashboard/Faqs")
    {
    }
}