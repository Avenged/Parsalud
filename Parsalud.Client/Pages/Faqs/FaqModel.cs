using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Faqs;

public class FaqModel
{
    public Guid? Id { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public bool Hidden { get; set; }

    public ManageFaqRequest ToRequest()
    {
        return new ManageFaqRequest
        {
            Question = Question ?? "",
            Answer = Answer ?? "",
            Hidden = Hidden,
        };
    }
}
