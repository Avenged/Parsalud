namespace Parsalud.BusinessLayer.Abstractions;

public class FaqSearchCriteria
{
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public int? MaxCount { get; set; }
    public Guid? ServiceId { get; set; }
    public string? ServiceCode { get; set; }
    public bool OnlyGeneric { get; set; }
}
