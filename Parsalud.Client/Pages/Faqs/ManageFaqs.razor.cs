using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Faqs;

public partial class ManageFaqs : ManagerBase<IFaqService, ParsaludFaq, ManageFaqRequest, FaqSearchCriteria>
{
    public ManageFaqs() : base("Dashboard/Faq/Create", "Nuevo FAQ")
    {    
    }
}
