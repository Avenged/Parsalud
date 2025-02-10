using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;
using System.Web;

namespace Parsalud.Client.Components;

public abstract class BaseAbm<Tkey> : BaseAbm, IDisposable
{
    [Parameter]
    public Tkey? Id { get; set; }

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

    //[Inject]
    //protected IApplicationConfiguration AppConfig { get; set; } = null!;

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