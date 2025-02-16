namespace Parsalud.BusinessLayer.Abstractions;

public class FaqSearchCriteria
{
    public string? Category { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public int? MaxCount { get; set; }
}
