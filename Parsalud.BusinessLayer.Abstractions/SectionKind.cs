namespace Parsalud.BusinessLayer.Abstractions;

[Serializable]
public readonly struct SectionKind
{
    public required string Name { get; init; }

    public static readonly SectionKind Page = "Página";
    public static readonly SectionKind Component = "Componente";
    public static readonly SectionKind[] Items = [Page, Component];

    public static implicit operator SectionKind(string name)
        => new()
        {
            Name = name,
        };

    public static implicit operator string(SectionKind sk)
        => sk.Name;

    public override string ToString()
    {
        return Name.ToString();
    }
}