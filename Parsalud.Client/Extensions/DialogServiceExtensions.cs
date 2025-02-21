namespace Radzen;

public static class DialogServiceExtensions
{
    public static async Task<bool> ConfirmCreationAsync(this DialogService ds)
    {
        var res = await ds.Confirm(
            message: "¿Estás seguro de que deseas crear este elemento?",
            title: "Atención",
            options: new ConfirmOptions
            {
                OkButtonText = "Confirmar",
                CancelButtonText = "Cancelar",
                CloseDialogOnOverlayClick = false,
                CloseDialogOnEsc = false
            });

        return res.GetValueOrDefault();
    }

    public static async Task<bool> ConfirmEditionAsync(this DialogService ds)
    {
        var res = await ds.Confirm(
            message: "¿Confirmas los cambios realizados? Asegúrate de que la información es correcta antes de continuar.",
            title: "Atención",
            options: new ConfirmOptions
            {
                OkButtonText = "Confirmar",
                CancelButtonText = "Cancelar",
                CloseDialogOnOverlayClick = false,
                CloseDialogOnEsc = false
            });

        return res.GetValueOrDefault();
    }

    public static async Task<bool> ConfirmDeletionAsync(this DialogService ds)
    {
        var res = await ds.Confirm(
            message: "¿Realmente quieres eliminar este elemento? Esta acción es irreversible.",
            title: "Atención",
            options: new ConfirmOptions
            {
                OkButtonText = "Confirmar",
                CancelButtonText = "Cancelar",
                CloseDialogOnOverlayClick = false,
                CloseDialogOnEsc = false
            });

        return res.GetValueOrDefault();
    }
}
