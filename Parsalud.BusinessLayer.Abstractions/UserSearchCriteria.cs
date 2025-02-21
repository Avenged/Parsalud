namespace Parsalud.BusinessLayer.Abstractions;

public class UserSearchCriteria
{
    public string? UserName { get; set; }
    public bool? LockoutEnabled { get; set; }
}
