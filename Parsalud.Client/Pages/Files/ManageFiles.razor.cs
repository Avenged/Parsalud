using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;
using Radzen;

namespace Parsalud.Client.Pages.Files;

public partial class ManageFiles : ManagerBase<IFileService, ParsaludFile, ManageFileRequest, FileSearchCriteria>
{
    public ManageFiles() : base("Dashboard/File/Create", "Subir archivo")
    {
    }

    private async Task DeleteByFileName(string fileName)
    {
        if (!await DS.ConfirmDeletionAsync())
        {
            return;
        }

        var response = await Service.DeleteByFileNameAsync(fileName);
        await AfterDeleteAsync(response);
    }
}
