using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Faqs;

public class FaqModel : IModelBase<FaqModel, ManageFaqRequest, ParsaludFaq>
{
    public Guid? Id { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public Guid? ServiceId { get; set; }
    public bool Hidden { get; set; }

    public FaqModel FromDto(ParsaludFaq dto)
    {
        return new FaqModel
        {
            Answer = dto.Answer,
            Hidden = dto.Hidden,
            Id = dto.Id,
            Question = dto.Question,
            ServiceId = dto.ServiceId,
        };
    }

    public ManageFaqRequest ToRequest()
    {
        return new ManageFaqRequest
        {
            Question = Question ?? "",
            Answer = Answer ?? "",
            Hidden = Hidden,
            ServiceId = ServiceId,
        };
    }
}
