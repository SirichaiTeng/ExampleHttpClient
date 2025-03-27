using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq.Protected;
using Moq;
using System.Net;
using UseWebApp.IServices;
using UseWebApp.Services;
using Castle.Core.Configuration;
using System.Text;

namespace UseWebApp.Tests;

public class ProgramTests
{
    [Fact]
    public async Task MaunTestAsync()
    {
        // ✅ 1. Mock HttpMessageHandler เพื่อให้ HttpClient ตอบกลับสำเร็จ
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"Data\": { \"Token\": \"mocked-token\" } }", Encoding.UTF8, "application/json") // ✅ JSON ถูกต้อง
            });

        // ✅ 2. สร้าง HttpClient จาก Mock Handler
        var mockHttpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://mocked-api.com/")
        };

        // ✅ 3. Mock IHttpClientFactory
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        mockHttpClientFactory
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(mockHttpClient);

        var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string>
    {
                { "Token:baseAddress", "https://mocked-api.com/" }
    })
    .Build();

        // ✅ 4. สร้าง ServiceCollection และแทนที่ HttpClientFactory
        var services = new ServiceCollection();
        services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(configuration);
        services.AddSingleton(mockHttpClientFactory.Object); // ใช้ HttpClient Mock
        services.AddScoped<ITokenService, TokenService>();   // Mock Service ที่ใช้ HttpClient
        services.AddScoped<ISendEmailService, SendEmailService>();

        // 🔥 แทนที่ DI ที่มาจาก Program.ConfigureServices()
        var serviceProvider = services.BuildServiceProvider();

        // ✅ 5. Resolve Service เพื่อทดสอบ HttpClient ถูกเรียกจริง
        var tokenService = serviceProvider.GetRequiredService<ITokenService>();
        await tokenService.GetToken(); // **กระตุ้นให้ HttpClient ถูกเรียก**

        Program.ConfigureServices(services, new ConfigurationBuilder().Build()); // โหลด Service เดิม

        // ✅ 6. ตรวจสอบว่า HttpClient ถูกเรียกอย่างน้อย 1 ครั้ง
        mockHandler.Protected().Verify(
            "SendAsync",
            Times.AtLeastOnce(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
        // ✅ 5. Run โปรแกรมหลัก (Main)
        var args = new[] { "test" };
        await Program.Main(args);


        // Assert
        Assert.Equal(0,Environment.ExitCode);
    }
}
