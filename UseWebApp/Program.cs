
using UseWebApp.Interfaces.IServices;
using UseWebApp.Services;

namespace UseWebApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = CreateApp(args);
        if (!args.Contains("test"))
        {
            await app.RunAsync();
        }
        else
        {
            Environment.ExitCode = 0;
        }
    }
    public static WebApplication CreateApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder.Services, builder.Configuration);
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();
        return app;
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITokenService,TokenService>();
        services.AddScoped<ISendEmailService,SendEmailService>();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddHttpClient("Token", client =>
        {
            client.BaseAddress = new Uri(configuration["Token:baseAddress"] ?? "");
        });

        services.AddHttpClient("SecretKey", client =>
        {
            client.BaseAddress = new Uri(configuration["SecretKey:baseAddress"] ?? "");
        });

        services.AddHttpClient("Email", client =>
        {
            client.BaseAddress = new Uri(configuration["Email:baseAddress"] ?? "");
        });
    }
}
