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
            Stopwatch start = Stopwatch.StartNew();
            start.Start();
            
            var url = "http://localhost:8000/";

            using (var client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                // Чтение файла в поток
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                
                start.Stop();
                Console.WriteLine($"GetDataRequest method1 completed in {start.ElapsedMilliseconds} ms.");
                start.Restart();
                start.Start();

                // Добавляем файл с нужным именем параметра — "file_bytes"
                form.Add(fileContent, "file_bytes", Path.GetFileName(filePath));
                
                start.Stop();
                Console.WriteLine($"GetDataRequest method2 completed in {start.ElapsedMilliseconds} ms.");
                start.Restart();
                start.Start();

                // Устанавливаем заголовок Accept
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                start.Stop();
                Console.WriteLine($"GetDataRequest method3 completed in {start.ElapsedMilliseconds} ms.");
                start.Restart();
                start.Start();

                // Отправка POST-запроса
                var response = await client.PostAsync(url, form);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                start.Stop();
                Console.WriteLine($"GetDataRequest method4 completed in {start.ElapsedMilliseconds} ms.");
                
                List<Root>? result = JsonSerializer.Deserialize<List<Root>>(responseContent);
                
                if (result != null)
                    return result;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        throw new NullReferenceException("Не удалось преобразовать данные");
    }
        
}