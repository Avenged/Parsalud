using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Parsalud.BusinessLayer.Abstractions;
using Radzen;
using Radzen.Blazor;

namespace Parsalud.Client.Components;

public abstract class BaseAbm<TModel, TService, TDto, TRequest, TCriteria>(string mainPath) : 
    BaseAbm, 
    IDisposable
    where TModel : class, IModelBase<TModel, TRequest, TDto>, new()
    where TService : class, IManagerServiceBase<TDto, TRequest, TCriteria>
    where TDto : class, IDtoBase
    where TRequest : class
    where TCriteria : class, new()
{
    protected RadzenTemplateForm<TModel> Form { get; set; } = default!;

    protected TModel Model { get; set; } = new();

    protected string MainPath { get; } = mainPath;

    [Inject]
    protected TService Service { get; set; } = default!;

    [Parameter]
    public Guid Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Id == Guid.Empty)
        {
            return;
        }

        var response = await Service.GetByIdAsync(Id);

        if (response.IsSuccessWithData)
        {
            Model = Model.FromDto(response.Data);
        }
    }

    public async Task Submit()
    {
        await GeneralSubmit(keep: false);
    }

    public async Task ManualSubmit()
    {
        if (!Form.IsValid)
        {
            Form.EditContext.Validate();
            NS.Notify(NotificationSeverity.Warning, "Revise los datos en su formulario");
            return;
        }

        var response = await GeneralSubmit(keep: true);

        if (response?.IsSuccess ?? false)
        {
            NS.Notify(NotificationSeverity.Success, "Cambios guardados");
        }
        else
        {
            NS.Notify(NotificationSeverity.Warning, summary: "No pudimos guardar los cambios", detail: response?.Message);
        }

        if (AbmAction == AbmAction.Create && (response?.IsSuccessWithData ?? false))
        {
            var newPath = NM.Uri.Replace("/Create", $"/{response.Data.Id}/Update");
            NM.NavigateTo(newPath);
        }
    }

    protected virtual async Task<BusinessResponse<TDto>?> GeneralSubmit(bool keep)
    {
        if (ReadOnly)
        {
            return null;
        }

        if (!keep && AbmAction == AbmAction.Create && !await DS.ConfirmCreationAsync())
        {
            return null;
        }
        else if (!keep && AbmAction == AbmAction.Update && !await DS.ConfirmEditionAsync())
        {
            return null;
        }

        BusinessResponse<TDto>? response;
        if (AbmAction == AbmAction.Create)
        {
            response = await Service.CreateAsync(Model.ToRequest());
        }
        else if (AbmAction == AbmAction.Update)
        {
            response = await Service.UpdateAsync(Id, Model.ToRequest());
        }
        else
        {
            throw new NotImplementedException();
        }

        if (!keep && response.IsSuccess)
        {
            NS.Notify(
                severity: NotificationSeverity.Success,
                summary: "Cambios guardados",
                detail: "",
                duration: TimeSpan.FromSeconds(5));

            NM.NavigateTo(MainPath);
        }
        else if (!keep && !response.IsSuccess)
        {
            NS.Notify(
                severity: NotificationSeverity.Warning,
                summary: "No pudimos guardar los cambios",
                detail: response.Message,
                duration: TimeSpan.FromSeconds(10));
        }

        return response;
    }

    protected void Discard()
    {
        NM.NavigateTo(MainPath);
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public abstract class BaseAbm() : ComponentBase
{
    protected bool ReadOnly { get; set; }
    protected bool IsBusy { get; set; }
    protected bool NotFound { get; set; }
    protected string? ErrorMessage { get; set; }

    [CascadingParameter]
    public PageStack Navigation { get; set; } = null!;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    [Inject]
    protected IJSRuntime JS { get; set; } = null!;

    [Inject]
    protected NavigationManager NM { get; set; } = null!;

    [Inject]
    protected DialogService DS { get; set; } = null!;

    [Inject]
    protected NotificationService NS { get; set; } = null!;

    [Parameter]
    public AbmAction AbmAction { get; set; }

    /// <summary>
    /// Indica si se debe mostrar el breadcrumb.
    /// </summary>
    [Parameter]
    public bool ShowBreadCrumb { get; set; } = true;

    /// <summary>
    /// Indica el modo en que fue abierto el ABM. Por defecto es Navigation.
    /// </summary>
    [Parameter]
    public OpenMode OpenMode { get; set; } = OpenMode.Navigation;

    [Parameter]
    public string? OverridePageName { get; set; }

    [Parameter]
    public string? OverridePageIcon { get; set; }

    [Parameter]
    public bool OverrideMode { get; set; }

    public bool IsModal => OpenMode == OpenMode.Modal || OpenMode == OpenMode.SideModal;

    public bool IsCreating => AbmAction == AbmAction.Create;
    public bool IsUpdating => AbmAction == AbmAction.Update;
    public bool IsViewing => AbmAction == AbmAction.View;

    protected async Task Return(object? result)
    {
        if (OpenMode == OpenMode.Stack && Navigation is not null)
        {
            await Navigation.Pop(result);
        }
        else if (OpenMode == OpenMode.Modal)
        {
            DS.Close(result);
        }
        else if (OpenMode == OpenMode.SideModal)
        {
            DS.CloseSide(result);
        }
        else if (OpenMode == OpenMode.Navigation)
        {
            //NM.NavigateTo(Resource);
        }
    }

    protected async Task Return(AbmResult? abmResult = default)
    {
        if (OpenMode == OpenMode.Stack && Navigation is not null)
        {
            await Navigation.Pop(abmResult);
        }
        else if (OpenMode == OpenMode.Modal)
        {
            DS.Close(abmResult);
        }
        else if (OpenMode == OpenMode.SideModal)
        {
            DS.CloseSide(abmResult);
        }
        else if (OpenMode == OpenMode.Navigation)
        {
            //NM.NavigateTo(Resource);
        }
    }

    private void VerifyAction()
    {
        if (OpenMode == OpenMode.Navigation)
        {
            try
            {
                AbmAction = NM.GetAbmAction();
            }
            catch (Exception)
            {
                AbmAction = AbmAction.Create;
            }
        }

        ReadOnly = AbmAction == AbmAction.View;
    }

    protected override void OnInitialized()
    {
        VerifyAction();
    }

    protected override void OnParametersSet()
    {
        VerifyAction();
    }

    protected string? Action(string? create = default, string? update = default, string? view = default)
    {
        switch (AbmAction)
        {
            case AbmAction.Create:
                return create;
            case AbmAction.Update:
                return update;
            case AbmAction.View:
                return view;
            default:
                break;
        }
        return null;
    }
}