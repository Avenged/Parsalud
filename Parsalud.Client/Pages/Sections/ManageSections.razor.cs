using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Sections;

public partial class ManageSections : ManagerBase<ISectionService, ParsaludSection, SectionSearchCriteria>
{
    public const string NEW_ITEM_TEXT = "Nueva sección";
    public const string CREATE_PATH = "Dashboard/Section/Create";

    public void CreateNew()
    {
        NM.NavigateTo(CREATE_PATH);
    }
}
