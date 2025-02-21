using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Users;

public partial class ManageUsers : ManagerBase<IUserManagerService, ParsaludUserDto, ManageUserRequest, UserSearchCriteria>
{
    public ManageUsers() : base("Dashboard/User/Create", "Nuevo usuario")
    {
    }
}