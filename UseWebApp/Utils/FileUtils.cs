using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace UseWebApp.Utils;

public static class FileUtils
{
    /// <summary>
    /// สำหรับ Mockdata ถ้ามี response เป็นไฟล์ json สามารถใช้ ReadJsonFileAsync ได้
    /// </summary>
    /// <typeparam">object.json</typeparam>
    /// <returns>T?</returns>
    public static async Task<T?> ReadJsonFileAsync<T>(string filepath)
    {
        using (FileStream fs = new FileStream(filepath,FileMode.Open, FileAccess.Read))
        {
            return await JsonSerializer.DeserializeAsync<T>(fs,new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, IncludeFields = true});
        }    
    }
    /// <summary>
    /// สำหรับข้อความ Templete 
    /// </summary>
    public static async Task<string> ReadTextFileAsync(string filepath)
    {
        using FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using StreamReader reader = new StreamReader(fs);
        return await reader.ReadToEndAsync();
    }
}
