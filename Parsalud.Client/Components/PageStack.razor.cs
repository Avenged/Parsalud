using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Reflection;

namespace Parsalud.Client.Components;

public partial class PageStack : ComponentBase
{
    const string MainId = "485b1dc2";
    private readonly Stack<Page> pages = [];
    private readonly List<TaskCompletionSource<dynamic>> tasks = [];

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback<CloseArgs> Close { get; set; }

    [Inject]
    public ILogger<PageStack> Logger { get; set; } = null!;

    protected override void OnInitialized()
    {
        NM.LocationChanged += HandleLocationChanged;
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        pages.Clear();
        StateHasChanged();
    }

    protected virtual void Dispose(bool disposing)
    {
        NM.LocationChanged -= HandleLocationChanged;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~PageStack()
    {
        Dispose(false);
    }

    public void Push(Type type, Dictionary<string, object?>? parameters = null)
    {
        // Cerrar cualquier tooltip que quedó visible para que no se vea superpuesto
        TS.Close();

        parameters ??= [];

        if (HasOpenModeProperty(type))
        {
            parameters.Remove(nameof(BaseAbm.OpenMode));
            parameters.Add(nameof(BaseAbm.OpenMode), OpenMode.Stack);
        }

        pages.Push(new Page(type, parameters));
        StateHasChanged();
    }

    public void Push<T>(Dictionary<string, object?>? parameters = null)
    {
        // Cerrar cualquier tooltip que quedó visible para que no se vea superpuesto
        TS.Close();

        parameters ??= [];

        if (HasOpenModeProperty<T>())
        {
            parameters.Remove(nameof(BaseAbm.OpenMode));
            parameters.Add(nameof(BaseAbm.OpenMode), OpenMode.Stack);
        }

        pages.Push(new Page(typeof(T), parameters));
        StateHasChanged();
    }

    public Task<dynamic> PushAsync(Type type, Dictionary<string, object?>? parameters = null)
    {
        // Cerrar cualquier tooltip que quedó visible para que no se vea superpuesto
        TS.Close();

        TaskCompletionSource<dynamic> task = new();
        tasks.Add(task);

        Push(type, parameters);

        return task.Task;
    }

    public Task<dynamic> PushAsync<T>(Dictionary<string, object?>? parameters = null)
    {
        // Cerrar cualquier tooltip que quedó visible para que no se vea superpuesto
        TS.Close();

        parameters ??= [];

        if (HasOpenModeProperty<T>())
        {
            parameters.Remove(nameof(BaseAbm.OpenMode));
            parameters.Add(nameof(BaseAbm.OpenMode), OpenMode.Stack);
        }

        TaskCompletionSource<dynamic> task = new();
        tasks.Add(task);

        Push<T>(parameters);

        return task.Task;
    }

    public async Task Pop(dynamic? result = null)
    {
        if (pages.Count == 0)
        {
            return;
        }

        bool exception = false;
        Page? page = default;

        try
        {
            page = pages.Pop();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Ocurrió un error al sacar una página del stack");
            exception = true;
        }

        if (!exception && page is not null)
        {
            try
            {
                await JS.InvokeAsync<bool>("animateStackPagePop", $"#{page.Id}", "fadeOutDown", "slower");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Ocurrió un error durante la invocación de la función JS 'animateStackPagePop'");
                exception = true;
            }
        }

        if (exception)
        {
            return;
        }

        if (page is not null)
        {
            await Close.InvokeAsync(new CloseArgs(page, result));
        }

        var task = tasks.LastOrDefault();
        if (task is not null && task.Task is not null && !task.Task.IsCompleted)
        {
            tasks.Remove(task);
            task.SetResult(result!);
        }
    }

    private static bool HasOpenModeProperty(Type type)
    {
        return type.GetProperty("OpenMode", BindingFlags.Public | BindingFlags.Instance) is not null;
    }

    private static bool HasOpenModeProperty<T>()
    {
        return typeof(T).GetProperty("OpenMode", BindingFlags.Public | BindingFlags.Instance) is not null;
    }

    static RenderFragment Renderer(Page page)
    {
        return new RenderFragment(builder =>
        {
            builder.OpenComponent(0, page.Type);

            if (page.Parameters is not null)
            {
                foreach (var parameter in page.Parameters)
                {
                    builder.AddAttribute(1, parameter.Key, parameter.Value);
                }
            }

            builder.CloseComponent();
        });
    }

    public class Page(Type type, Dictionary<string, object?>? parameters)
    {
        public string Id { get; } = HtmlHelper.GenerateId("Stack page");
        public Type Type { get; init; } = type;
        public Dictionary<string, object?>? Parameters { get; init; } = parameters;
    }

    public class CloseArgs(PageStack.Page page, dynamic? result)
    {
        public Page Page { get; init; } = page;
        public dynamic? Result { get; init; } = result;
    }
}