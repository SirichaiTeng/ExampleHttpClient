namespace UseWebApp.Interfaces.IServices;

public interface ITokenService
{
    Task<string> GetToken();
    Task<string?> GetKey();
}
