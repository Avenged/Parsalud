using Microsoft.AspNetCore.Identity;

namespace Parsalud;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError InvalidEmail(string? email)
    {
        return new IdentityError { Code = nameof(InvalidEmail), Description = $"El email '{email}' no es correcto." };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError { Code = nameof(DuplicateEmail), Description = $"El nombre de usuario '{email}' ya ha sido tomado." };
    }

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
