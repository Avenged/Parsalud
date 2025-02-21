using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;
using Radzen;

namespace Parsalud.Client.Pages.Files;

public partial class ManageFile : BaseAbm<FileModel, IFileService, ParsaludFile, ManageFileRequest, FileSearchCriteria>
{
    public ManageFile() : base("Dashboard/Files")
    {  
    }

    private int progress = 0;

    private void Error(UploadErrorEventArgs args)
    {
        NS.Notify(
            severity: NotificationSeverity.Warning,
            summary: "No se pudo subir el archivo",
            detail: args.Message,
            duration: TimeSpan.FromSeconds(10));
    }

    private void Complete(UploadCompleteEventArgs args)
    {
        NS.Notify(
            severity: NotificationSeverity.Success,
            summary: "Archivo subido con éxito",
            detail: "",
            duration: TimeSpan.FromSeconds(5));

        Discard();
    }
}
