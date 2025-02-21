namespace Parsalud.BusinessLayer.Abstractions;

public class PostSearchCriteria
{
    public Guid[]? ExcludedIds { get; set; }
    public Guid[]? Ids { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool IncludeHidden { get; set; }
    public Guid[]? CategoryIds { get; set; }
    public int? Page { get; set; }
    public int? Size { get; set; }
}