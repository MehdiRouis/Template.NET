using Template.Core.Domain.Errors.Common;

namespace Template.Core.Domain.Errors.Authentication;

public static class SignupErrors
{
    public static readonly DomainError Success = new("auth.signup.success", "Compte créé avec succès.");
    public static readonly DomainError InvalidEmail = new("auth.signup.invalid_email", "Adresse email invalide.");

    public static readonly DomainError WeakPassword = new("auth.signup.weak_password", "Mot de passe trop faible.");

    public static readonly DomainError InvalidPhone = new("auth.signup.invalid_phone", "Numéro de téléphone invalide.");

    public static readonly DomainError InvalidName = new("auth.signup.invalid_name", "Nom invalide.");

    public static readonly DomainError EmailAlreadyExists = new("auth.signup.email_already_exists", "Un compte existe déjà.");
}
