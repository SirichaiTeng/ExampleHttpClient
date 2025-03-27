namespace UseWebApp.Models.Response;

public class ResponseToken
{
    public TokenData? Data { get; set; }
}

public class TokenData
{
    public string? Token { get; set; }

    public string? Exp { get; set; }
}
