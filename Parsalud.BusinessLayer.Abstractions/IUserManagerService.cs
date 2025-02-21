using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IUserManagerService : IManagerServiceBase<ParsaludUserDto, ManageUserRequest, UserSearchCriteria>
{
}
