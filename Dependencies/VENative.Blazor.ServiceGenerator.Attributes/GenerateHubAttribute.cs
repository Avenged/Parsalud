using System;

namespace VENative.Blazor.ServiceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class GenerateHubAttribute(string? @namespace = default, string? route = default, bool useAuthentication = false) : Attribute
{
    public string? Route { get; } = route;
    public string? Namespace { get; } = @namespace;
    public bool UseAuthentication { get; } = useAuthentication;
}