using System.Text.Json;
using System.Text;
using UseWebApp.IServices;
using UseWebApp.Models.Entities;

namespace UseWebApp.Services;

public class SendEmailService : ISendEmailService
{
    private readonly IConfiguration _configuration1;
    private readonly HttpClient _emailHttpClient;
    private readonly ITokenService _tokenService;
    public SendEmailService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ITokenService tokenService)
    {
        _configuration1 = configuration;
        _emailHttpClient = httpClientFactory.CreateClient("Email");
        _tokenService = tokenService;
    }

    public async Task<bool> SendEmail(List<EmployeeSport> employeeSports)
    {
        var employeeNameNoGold = employeeSports.Where(count => count.Gold == 0).Select(name => name.Name).ToList();
        var emailpayload = new EmailPayload
        {
            From = "InwZa55@hotmail.com"
        };
        emailpayload.Subject = "EIEIZAMAK";
        emailpayload.To = "mylove.1@hotmail.com";
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine("<style>");
        sb.AppendLine("body { font-family: Arial, sans-serif; font-size: 14px; }");
        sb.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 10px; }");
        sb.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
        sb.AppendLine("th { background-color: #f4f4f4; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<p>เรียนผู้เกี่ยวข้อง,</p>");
        sb.AppendLine("<p>ขอแจ้งรายชื่อพนักงานที่ยังไม่ได้รับเหรียญทองจากการแข่งขันกีฬา:</p>");
        sb.AppendLine("<table>");
        sb.AppendLine("<tr><th>ลำดับ</th><th>ชื่อพนักงาน</th></tr>");

        int index = 1;
        foreach (var name in employeeNameNoGold)
        {
            sb.AppendLine($"<tr><td>{index}</td><td>{name}</td></tr>");
            index++;
        }

        sb.AppendLine("</table>");
        sb.AppendLine("<p>กรุณาติดต่อผู้ที่เกี่ยวข้องเพื่อดำเนินการต่อไป</p>");
        sb.AppendLine("<p>ขอแสดงความนับถือ,</p>");
        sb.AppendLine("<p>ฝ่ายบริหาร</p>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        emailpayload.Body = sb.ToString();
        string? token = await _tokenService.GetToken();

        var request = new HttpRequestMessage(HttpMethod.Post, _emailHttpClient.BaseAddress);
        request.Headers.Add("ClientId", _configuration1["Email:clientId"] ?? "");
        request.Headers.Add("access_token", token);
        request.Content = new StringContent(JsonSerializer.Serialize(emailpayload),Encoding.UTF8,"application/json");
        var response = await _emailHttpClient.SendAsync(request);
        return response.IsSuccessStatusCode;          
    }
}
