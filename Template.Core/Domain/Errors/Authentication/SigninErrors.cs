using Template.Core.Domain.Errors.Common;

namespace Template.Core.Domain.Errors.Authentication;

public static class SigninErrors
{
    public static readonly DomainError Success = new("auth.signin.success", "Connexion réussie.");
    public static readonly DomainError InvalidRequest = new("auth.signin.invalid_request", "Requête invalide.");

    public static readonly DomainError InvalidCredentials = new("auth.signin.invalid_credentials", "Identifiants invalides.");

    public static readonly DomainError AccountDisabled = new("auth.signin.account_disabled", "Compte désactivé.");
}