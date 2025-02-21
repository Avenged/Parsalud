using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Sections;

public partial class ManageSections : ManagerBase<ISectionService, ParsaludSection, ManageSectionRequest, SectionSearchCriteria>
{
    public ManageSections() : base("Dashboard/Section/Create", "Nueva sección")
    {      
    }
}
