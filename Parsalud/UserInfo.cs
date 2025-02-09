namespace Parsalud;

// Add properties to this class and update the server and client AuthenticationStateProviders
// to expose more information about the authenticated user to the client.
public class UserInfo
{
    public required string NameIdentifier { get; init; }
    public required string Name { get; init; }
    public required string? GivenName { get; init; }
    public required string? Surname { get; init; }
    public required string? MobilePhone { get; init; }
    public required string? HomePhone { get; init; }
    public required string Email { get; init; }
    public required string[] Roles { get; init; }
}