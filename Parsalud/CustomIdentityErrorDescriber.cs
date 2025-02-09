using Microsoft.AspNetCore.Identity;

namespace Parsalud.BusinessLayer;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError()
    {
        return new IdentityError { Code = nameof(DefaultError), Description = "Ha ocurrido un error desconocido." };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError { Code = nameof(PasswordTooShort), Description = $"La contraseña debe tener al menos {length} caracteres." };
    }

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError { Code = nameof(DefaultError), Description = "Contraseña incorrecta." };
    }
}
