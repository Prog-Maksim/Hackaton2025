using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows;
using report.Models;

namespace report.Scripts;

public static class GetDataRequest
{
    public static async Task<List<Root>> GetDataRequestMethod(string filePath)
    {
        try
        {
            var url = "http://localhost:8000/";

            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                
                form.Add(fileContent, "file_bytes", Path.GetFileName(filePath));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var response = await client.PostAsync(url, form);
                var responseContent = await response.Content.ReadAsStringAsync();

                List<Root>? result = JsonSerializer.Deserialize<List<Root>>(responseContent);

                if (result != null)
                    return result;
            }
        }
        catch (HttpRequestException error)
        {
            throw new NullReferenceException("Ошибка отправки запроса. \n\nЕсли вы используете прокси, отключите!");
        }
        
        throw new NullReferenceException("Произошла неизвестная ошибка");
    }
        
}