using System;

namespace VENative.Blazor.ServiceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public class GenerateClientAttribute(string @namespace = "{interfaceNamespace}", string? route = "/{interfaceName}", string? generatedClassPrefix = "Client", string? generatedClassName = "{generatedClassPrefix}{interfaceName}") : Attribute
{
    public string Namespace { get; } = @namespace;
    public string? Route { get; } = route;
    public string? GeneratedClassPrefix { get; } = generatedClassPrefix;
    public string? GeneratedClassName { get; } = generatedClassName;
}