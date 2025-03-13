using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Services;

public partial class ManageService : BaseAbm<ServiceModel, IServiceService, ParsaludService, ManageServiceRequest, ServiceSearchCriteria>
{
    public ManageService() : base("Dashboard/Services")
    {
    }
}