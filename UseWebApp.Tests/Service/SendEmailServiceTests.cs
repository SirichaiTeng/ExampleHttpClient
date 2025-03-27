using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using UseWebApp.IServices;
using UseWebApp.Models.Entities;
using UseWebApp.Services;

namespace UseWebApp.Tests.Service;


public class SendEmailServiceTests
{
    [Fact]
    public async Task sendEmail_Should_ReturnSuccess()
    {
        // Arrange
        var request = new List<EmployeeSport>
        {
            new EmployeeSport {
                Name = "name1",
                LastName = "name1",
                Bronze = 1,
                Silver = 1,
                Gold = 1,
            },
            new EmployeeSport {
                Name = "name2",
                LastName = "name2",
                Bronze = 1,
                Silver = 1,
                Gold = 0,
            },
            new EmployeeSport {
                Name = "name3",
                LastName = "name3",
                Bronze = 1,
                Silver = 1,
                Gold = 0,
            },
            new EmployeeSport {
                Name = "name4",
                LastName = "name4",
                Bronze = 1,
                Silver = 1,
                Gold = 1,
            },

        };

        var mockTokenService = new Mock<ITokenService>();
        mockTokenService.Setup(getToken => getToken.GetToken()).ReturnsAsync("faketoken");

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(config => config["Email:clientId"]).Returns("IDIDIDID");

        #region ตัวอย่างที่ซับซ้อน By Claude
        //// สร้าง mock ของ HttpMessageHandler
        //var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        //// กำหนดพฤติกรรมซับซ้อนสำหรับการจำลองการยืนยันตัวตน (Authentication)
        //mockHttpMessageHandler
        //    .Protected()
        //    // ตั้งค่าให้ method SendAsync
        //    .Setup<Task<HttpResponseMessage>>(
        //        "SendAsync",
        //        // กำหนดเงื่อนไขซับซ้อนสำหรับ HttpRequestMessage
        //        ItExpr.Is<HttpRequestMessage>(req =>
        //            // เช็คว่าเป็น POST request ไปยัง endpoint การล็อกอิน
        //            req.Method == HttpMethod.Post &&
        //            req.RequestUri.PathAndQuery == "/api/auth/login" &&

        //            // ตรวจสอบว่ามี headers ที่จำเป็น
        //            req.Headers.Contains("X-API-Key") &&
        //            req.Headers.Accept.Any(h => h.MediaType == "application/json") &&

        //            // ตรวจสอบประเภทของเนื้อหาและรูปแบบ
        //            req.Content is StringContent &&
        //            req.Content.Headers.ContentType.MediaType == "application/json" &&

        //            // ตรวจสอบข้อมูลใน JSON body ว่ามีฟิลด์ที่ถูกต้อง
        //            new Func<bool>(() => {
        //                // อ่านเนื้อหา request ออกมา
        //                string content = req.Content.ReadAsStringAsync().Result;

        //                // แปลง JSON เป็น dynamic object เพื่อตรวจสอบค่า
        //                dynamic jsonObj = JsonConvert.DeserializeObject(content);

        //                // ตรวจสอบว่ามีฟิลด์ username และ password
        //                return jsonObj.username != null &&
        //                       jsonObj.username.ToString() == "testuser" &&
        //                       jsonObj.password != null &&
        //                       jsonObj.password.ToString().Length >= 8;
        //            })()
        //        ),
        //        // ยอมรับ CancellationToken ใดๆ
        //        ItExpr.IsAny<CancellationToken>())
        //    // กำหนดค่าที่จะส่งกลับเมื่อมีการเรียกใช้ SendAsync ที่ตรงตามเงื่อนไข
        //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        // สร้าง response content เป็น token ในรูปแบบ JSON
        //        Content = new StringContent(
        //            JsonConvert.SerializeObject(new
        //            {
        //                token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        //                expires_in = 3600
        //            }),
        //            Encoding.UTF8,
        //            "application/json"
        //        ),
        //        // กำหนด headers ให้กับ response
        //        Headers = {
        //{ "X-Rate-Limit-Remaining", "99" },
        //{ "X-Request-ID", "abc123" }
        //        }
        //    });

        //// กำหนดพฤติกรรมแยกอีกอันสำหรับกรณีที่ token ไม่ถูกต้อง
        //mockHttpMessageHandler
        //    .Protected()
        //    .Setup<Task<HttpResponseMessage>>(
        //        "SendAsync",
        //        // กำหนดเงื่อนไขสำหรับ request ที่มี Authorization header แต่ token ไม่ถูกต้อง
        //        ItExpr.Is<HttpRequestMessage>(req =>
        //            req.Method == HttpMethod.Get &&
        //            req.RequestUri.PathAndQuery.StartsWith("/api/protected") &&
        //            req.Headers.Authorization != null &&
        //            req.Headers.Authorization.Parameter != "validToken123"
        //        ),
        //        ItExpr.IsAny<CancellationToken>())
        //    // คืนค่า 401 Unauthorized สำหรับ token ที่ไม่ถูกต้อง
        //    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        //// สร้าง HttpClient จาก mock handler
        //var client = new HttpClient(mockHttpMessageHandler.Object)
        //{
        //    BaseAddress = new Uri("https://api.example.com")
        //};
        #endregion

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()                                                                            // .Protected() ใช้เข้าถึง protected method ของ HttpMessageHandler 
            .Setup<Task<HttpResponseMessage>>(                                                      // .Setup<Task<HttpResponseMessage>> กำหนดการตอบกลับของ method
                "SendAsync",                                                                        // "SendAsync" คือชื่อ method ที่ต้องการจำลอง
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)                                // กำหนดค่าที่จะคืนกลับเมื่อมีการเรียกใช้ SendAsync method
            {
                Content = new StringContent(JsonConvert.SerializeObject("{\"success\": true}"))
            });

        var HttpClient = new HttpClient(mockHttpMessageHandler.Object)                              // สร้าง HttpClient ตัวจริง โดยใช้ mock HttpMessageHandler ที่เตรียมไว้
        {
            BaseAddress = new Uri("https://example.com")
        };

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();                                 
        mockHttpClientFactory.Setup(factory => factory.CreateClient("Email")).Returns(HttpClient);  // กำหนด HttpClientFactory ให้เมื่อเรียกใช้ CreateClient("Email") จะคืนค่า HttpClient ที่สร้างไว้ก่อนหน้า

        var emailService = new SendEmailService(mockConfig.Object, mockHttpClientFactory.Object, mockTokenService.Object);

        // Act
        var response = await emailService.SendEmail(request);

        // Assert
        Assert.True(response);



    }
}
