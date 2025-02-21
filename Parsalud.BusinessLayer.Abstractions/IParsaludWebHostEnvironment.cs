namespace Parsalud.BusinessLayer.Abstractions;

public interface IParsaludWebHostEnvironment
{
    /// <summary>
    /// Gets the absolute path to the directory that contains the web-servable application content files.
    /// This defaults to the 'wwwroot' subfolder.
    /// </summary>
    string WebRootPath { get; }
}
