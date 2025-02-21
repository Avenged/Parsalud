using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.StyleSheets;

public partial class ManageStyleSheets : ManagerBase<IStyleSheetService, ParsaludStyleSheet, StyleSheetSearchCriteria>
{
    public const string NEW_ITEM_TEXT = "Nueva hoja de estilo";
    public const string CREATE_PATH = "Dashboard/StyleSheet/Create";

    public void CreateNew()
    {
        NM.NavigateTo(CREATE_PATH);
    }
}
