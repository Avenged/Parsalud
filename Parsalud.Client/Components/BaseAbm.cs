using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;
using System.Web;

namespace Parsalud.Client.Components;

public abstract class BaseAbm<Tkey, TModel> : BaseAbm, IDisposable
    where TModel : class
{
    protected override sealed async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // Ensures the user has the required role to make changes in the ABM
        TaskCompletionSource tcs = new();
        var state = await AuthenticationStateTask;

        var editRoleSpecified = !string.IsNullOrWhiteSpace(EditRole);
        var hasRequiredRole = state.User.IsInRole(EditRole ?? "");
        var isEditAction = AbmAction != AbmAction.View;

        if (editRoleSpecified && !hasRequiredRole && isEditAction)
        {
            var encodedUrl = HttpUtility.UrlEncode(NM.Uri);
            NM.NavigateTo($"Unauthorized/{encodedUrl}", replace: true);
            await tcs.Task;
        }

        await OnParametersSetAsyncImpl();
    }

    protected abstract Task OnParametersSetAsyncImpl();

    protected BaseAbm(string resource, string singularResourceName, string? editRole = default) : base(resource, singularResourceName, editRole)
    {
    }

    [Parameter]
    public Tkey? Id { get; set; }

    protected TModel? Model { get; set; }

#pragma warning disable CS0649
    private DotNetObjectReference<BaseAbm<Tkey, TModel>>? reference;
#pragma warning restore CS0649

    public virtual void Dispose()
    {
        reference?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public abstract class BaseAbm(string resource, string singularResourceName, string? editRole = default) : ComponentBase
{
    protected readonly string? EditRole = editRole;
    protected readonly string Resource = resource;
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

    //[Inject]
    //protected IApplicationConfiguration AppConfig { get; set; } = null!;

    [Parameter]
    public AbmAction AbmAction { get; set; }

    [Parameter]
    public string SingularResourceName { get; set; } = singularResourceName;

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
            NM.NavigateTo(Resource);
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
            NM.NavigateTo(Resource);
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
}