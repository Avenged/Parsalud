using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;
using Radzen;

namespace Parsalud.Client.Components;

public class ManagerBase<TService, TDto, TCriteria> : ComponentBase
    where TService : class, IManagerServiceBase<TDto, TCriteria>
    where TDto : class
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
    protected NavigationManager NM { get; set; } = default!;

    protected async virtual Task Delete(Guid id)
    {
        if (!await DS.ConfirmDeletionAsync())
        {
            return;
        }

        await Service.DeleteAsync(id);
        await Submit();
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