using UseWebApp.Utils;
namespace UseWebApp.Constants;

public class EmailConstant
{
    //ทำการเรียกใช้งาน Template/BodySendFile
    public static string BodySendFile(string folder) => FileUtils.ReadTextFileAsync("Template/BodySendFile").Result.Replace("{folder}", folder);

    public static string SubjectSendFile(string folder) => FileUtils.ReadTextFileAsync("Template/SubjectSendFile").Result.Replace("{folder}", folder);
}
