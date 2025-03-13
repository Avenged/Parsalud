using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IServiceService : IManagerServiceBase<ParsaludService, ManageServiceRequest, ServiceSearchCriteria>
{
}