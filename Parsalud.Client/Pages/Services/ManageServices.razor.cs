using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Services;

public partial class ManageServices : ManagerBase<IServiceService, ParsaludService, ManageServiceRequest, ServiceSearchCriteria>
{
    public ManageServices() : base("Dashboard/Service/Create", "Nuevo servicio")
    {
    }
}