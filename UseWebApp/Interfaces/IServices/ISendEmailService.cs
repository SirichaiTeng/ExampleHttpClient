using UseWebApp.Models.Entities;

namespace UseWebApp.Interfaces.IServices;

public interface ISendEmailService
{
    Task<bool> SendEmail(List<EmployeeSport> employeeSports);
}
