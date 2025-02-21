using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Faqs;

public partial class ManageFaqs : ManagerBase<IFaqService, ParsaludFaq, FaqSearchCriteria>
{
    public const string NEW_ITEM_TEXT = "Nuevo FAQ";
    public const string CREATE_PATH = "Dashboard/Faq/Create";

    public void CreateNew()
    {
        NM.NavigateTo(CREATE_PATH);
    }
}
