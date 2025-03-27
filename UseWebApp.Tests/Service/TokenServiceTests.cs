using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using UseWebApp.Models.Response;
using UseWebApp.Services;

namespace UseWebApp.Tests.Service;

public class TokenServiceTests
{
    [Fact]
    public async Task GetToken_ReturnToken()
    {
        // Arrange
        var expectedToken = "token";
        var tokenResponse = new ResponseToken
        {
            Data = new TokenData { Token = expectedToken, Exp = "1150" }
        };

        var mockClientId = new Mock<IConfigurationSection>();
        mockClientId.Setup(config => config.Key).Returns("clientId");
        mockClientId.Setup(config => config.Value).Returns("12341234");

        var mockSecretName = new Mock<IConfigurationSection>();
        mockSecretName.Setup(config => config.Key).Returns("secretName");
        mockSecretName.Setup(config => config.Value).Returns("DevJaaaa");

        var mockSettings = new Mock<IConfigurationSection>();
        mockSettings.Setup(children => children.GetChildren()).Returns(new List<IConfigurationSection> { mockClientId.Object, mockSecretName.Object });

        var mockConfigure = new Mock<IConfiguration>();
        mockConfigure.Setup(config => config.GetSection("Token")).Returns(mockSettings.Object);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(tokenResponse))
            });

        var httpclient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://example.com")
        };
        var mockIHttpClieny = new Mock<IHttpClientFactory>();
        mockIHttpClieny.Setup(factory => factory.CreateClient("Token")).Returns(httpclient);
        var tokenService = new TokenService(mockIHttpClieny.Object, mockConfigure.Object);

        // Act
        var result = await tokenService.GetToken();

        // Assert
        Assert.Equal(expectedToken, result);
    }

    [Fact]
    public async Task GeyKey_ShouldReturnKey()
    {
        // Arrange
        var mockResponse = new SecretKeyResponse
        {
            Data = "1234",
            Message = "",
            Success = true,
        };
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(config => config["Token:clientId"]).Returns("clientId");
        mockConfig.Setup(config => config["Token:secretName"]).Returns("DEvT");

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockResponse))
            });

        var httpclient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://example.com")
        };
        var mockIHttpClieny = new Mock<IHttpClientFactory>();
        mockIHttpClieny.Setup(factory => factory.CreateClient("SecretKey")).Returns(httpclient);
        var tokenService = new TokenService(mockIHttpClieny.Object, mockConfig.Object);

        // Act
        var response = await tokenService.GetKey();

        // Assert
        Assert.Equal(response,"1234");



    }
}
