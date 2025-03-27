using UseWebApp.Models.Entities;

namespace UseWebApp.IServices;

public interface ISendEmailService
{
    Task<bool> SendEmail(List<EmployeeSport> employeeSports);
}
