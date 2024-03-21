namespace Movies.Api.Auth;

public static class AuthConstants
{
    // -- Admin --
    public const string AdminUserPolicyName = "Admin";
    public const string AdminUserClaimName = "admin";
    
    // -- Trusted member --
    public const string TrustedUserPolicyName = "TrustedUser";
    public const string TrustedUserClaimName = "trusted_member";
}