using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;
using Radzen;

namespace Parsalud.Client.Components;

public class ManagerBase<TService, TDto, TRequest, TCriteria>(string createPath, string newItemText) : ComponentBase
    where TService : class, IManagerServiceBase<TDto, TRequest, TCriteria>
    where TDto : class
    where TRequest : class
    where TCriteria : class, new()
{
    protected TCriteria Criteria { get; set; } = new();

    protected string? ErrorMessage { get; set; }

    protected bool IsSearching { get; set; }

    protected TDto[]? Items { get; set; }

    [Inject]
    protected TService Service { get; set; } = default!;

    [Inject]
    protected DialogService DS { get; set; } = default!;

    [Inject]
    protected NotificationService NS { get; set; } = default!;

    [Inject]
    protected NavigationManager NM { get; set; } = default!;
    public string CREATE_PATH { get; } = createPath;
    public string NEW_ITEM_TEXT { get; } = newItemText;

    public void CreateNew()
    {
        NM.NavigateTo(CREATE_PATH);
    }

    protected void Reset()
    {
        ErrorMessage = null;
        Items = null;
        Criteria = new();
    }

    protected async virtual Task Delete(Guid id)
    {
        if (!await DS.ConfirmDeletionAsync())
        {
            return;
        }

        var response = await Service.DeleteAsync(id);
        await AfterDeleteAsync(response);
    }

    protected virtual async Task AfterDeleteAsync(BusinessResponse response)
    {
        if (response.IsSuccess)
        {
            NS.Notify(
                severity: NotificationSeverity.Success,
                summary: "Elemento eliminado",
                detail: "",
                duration: TimeSpan.FromSeconds(5));
            await Submit();
        }
        else
        {
            NS.Notify(
                severity: NotificationSeverity.Warning,
                summary: "No pudimos eliminar el elemento",
                detail: response.Message,
                duration: TimeSpan.FromSeconds(10));
        }
    }

    protected async virtual Task Submit()
    {
        Items = null;
        IsSearching = true;
        await Task.Yield();

        var response = await Service.GetByCriteriaAsync(Criteria);

        if (response.IsSuccessWithData)
        {
            Items = response.Data?.Data;
        }
        else
        {
            ErrorMessage = response.Message;
        }

        IsSearching = false;
    }
}