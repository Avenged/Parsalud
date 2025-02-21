using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.StyleSheets;

public partial class ManageStyleSheets : ManagerBase<IStyleSheetService, ParsaludStyleSheet, ManageStyleSheetRequest, StyleSheetSearchCriteria>
{
    public ManageStyleSheets() : base("Dashboard/StyleSheet/Create", "Nueva hoja de estilo")
    {     
    }
}
