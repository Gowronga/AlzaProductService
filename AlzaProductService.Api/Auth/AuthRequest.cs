namespace AlzaProductService.Api.Auth;

public record AuthRequest(
    string ClientId,
    string ClientSecret
);
