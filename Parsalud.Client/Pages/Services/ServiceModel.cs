using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Services;

public class ServiceModel : IModelBase<ServiceModel, ManageServiceRequest, ParsaludService>
{
    public string? Name { get; set; }
    public string? Code { get; set; }

    public ServiceModel FromDto(ParsaludService dto)
    {
        return new ServiceModel
        {
            Name = dto.Name,
            Code = dto.Code,
        };
    }

    public ManageServiceRequest ToRequest()
    {
        return new ManageServiceRequest(Name ?? "", Code ?? "");
    }
}