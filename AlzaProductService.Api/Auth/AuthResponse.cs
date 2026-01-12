namespace AlzaProductService.Api.Auth;

public record AuthResponse(
    string AccessToken,
    int ExpiresIn
);