using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IFaqService : IManagerServiceBase<ParsaludFaq, ManageFaqRequest, FaqSearchCriteria>
{
}