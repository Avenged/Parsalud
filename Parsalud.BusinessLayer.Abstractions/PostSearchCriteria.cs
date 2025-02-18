namespace Parsalud.BusinessLayer.Abstractions;

public class PostSearchCriteria
{
    public Guid[]? Ids { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public Guid[]? CategoryIds { get; set; }
    public int? Page { get; set; }
    public int? Size { get; set; }
}