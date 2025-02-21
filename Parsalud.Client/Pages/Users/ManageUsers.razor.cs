using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;
using Radzen;

namespace Parsalud.Client.Pages.Users;

public partial class ManageUsers : ManagerBase<IUserManagerService, ParsaludUserDto, ManageUserRequest, UserSearchCriteria>
{
    public ManageUsers() : base("Dashboard/User/Create", "Nuevo usuario")
    {
    }

    public static async Task<bool> ConfirmUnlockAsync(DialogService ds, string userName)
    {
        var res = await ds.Confirm(
            message: $"¿Realmente quieres habilitar al usuario '{userName}'?",
            title: "Atención",
            options: new ConfirmOptions
            {
                OkButtonText = "Habilitar",
                CancelButtonText = "Cancelar",
                CloseDialogOnOverlayClick = false,
                CloseDialogOnEsc = false
            });

        return res.GetValueOrDefault();
    }

    public static async Task<bool> ConfirmLockAsync(DialogService ds, string userName)
    {
        var res = await ds.Confirm(
            message: $"¿Realmente quieres deshabilitar al usuario '{userName}'? El usuario ya no podrá iniciar sesión",
            title: "Atención",
            options: new ConfirmOptions
            {
                OkButtonText = "Deshabilitar",
                CancelButtonText = "Cancelar",
                CloseDialogOnOverlayClick = false,
                CloseDialogOnEsc = false
            });

        return res.GetValueOrDefault();
    }

    private async Task Lock(ParsaludUserDto user)
    {
        if (!await ConfirmLockAsync(DS, user.UserName))
        {
            return;
        }

        var response = await Service.UpdateAsync(user.Id, new ManageUserRequest
        {
            IsDisabled = true,
        });

        if (response.IsSuccess)
        {
            NS.Notify(
                severity: NotificationSeverity.Success,
                summary: "Usuario deshabilitado",
                detail: "",
                duration: TimeSpan.FromSeconds(5));
        }
        else
        {
            NS.Notify(
                severity: NotificationSeverity.Warning,
                summary: "No pudimos deshabilitar el usuario",
                detail: response.Message,
                duration: TimeSpan.FromSeconds(10));
            return;
        }

        await Submit();
    }

    private async Task Unlock(ParsaludUserDto user)
    {
        if (!await ConfirmUnlockAsync(DS, user.UserName))
        {
            return;
        }

        var response = await Service.UpdateAsync(user.Id, new ManageUserRequest
        {
            IsDisabled = false,
        });

        if (response.IsSuccess)
        {
            NS.Notify(
                severity: NotificationSeverity.Success,
                summary: "Usuario habilitado",
                detail: "",
                duration: TimeSpan.FromSeconds(5));
        }
        else
        {
            NS.Notify(
                severity: NotificationSeverity.Warning,
                summary: "No pudimos habilitar el usuario",
                detail: response.Message,
                duration: TimeSpan.FromSeconds(10));
            return;
        }

        await Submit();
    }
}