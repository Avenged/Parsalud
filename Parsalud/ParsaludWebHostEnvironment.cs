using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud;

public class ParsaludWebHostEnvironment(string webRootPath) : IParsaludWebHostEnvironment
{
    public string WebRootPath => webRootPath;
}
