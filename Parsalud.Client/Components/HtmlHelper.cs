using System.Text.RegularExpressions;

namespace Parsalud.Client.Components;

public static class HtmlHelper
{
    private static readonly Random _random = new();

    /// <summary>
    /// Genera un ID válido para HTML.
    /// </summary>
    /// <param name="baseName"></param>
    /// <returns></returns>
    public static string GenerateId(string baseName)
    {
        string validBaseName = Regex.Replace(baseName, @"[^a-zA-Z0-9_-]", "-");
        if (!Regex.IsMatch(validBaseName, @"^[a-zA-Z_]"))
        {
            validBaseName = "_" + validBaseName;
        }

        string uniqueSuffix = _random.Next(1000, 9999).ToString();
        return $"{validBaseName}-{uniqueSuffix}";
    }
}
