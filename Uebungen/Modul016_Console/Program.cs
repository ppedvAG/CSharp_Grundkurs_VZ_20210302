using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Lab16_Console
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static string apiUrl = "api/todoitems/";
        static void Main(string[] args)
        {
            client.BaseAddress = new Uri("https://meineurl/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            TodoItem item = new TodoItem();
            item.Name = "Mein neues Item";
            item.IsComplete = false;

            try
            {
                Uri url = Create(item);
                Console.WriteLine($"Erstellt mit URL {url}");

                item = Get(url.PathAndQuery);
                Console.WriteLine(item.Info());

                item.Name = "Ich habe einen neuen Namen!";
                item = Update(item);

                Console.WriteLine(item.Info());

                Console.ReadLine();

                HttpStatusCode code = Delete(item.Id);
                Console.WriteLine($"Wurde gelöscht mit Code: {code}");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        static string CreateJson(TodoItem item)
        {
            return JsonSerializer.Serialize(item, typeof(TodoItem));
        }

        static TodoItem CreateTodoItem(string json)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<TodoItem>(json, options);
            }
            catch
            {
                return null;
            }
        }

        static List<TodoItem> CreateTodoItems(string json)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<TodoItem>>(json, options);
            }
            catch
            {
                return null;
            }
        }

        static TodoItem Get(long id)
        {
            return Get($"{apiUrl}{id}");
        }

        static TodoItem Get(string path)
        {
            TodoItem item = null;

            HttpResponseMessage response = client.GetAsync(path).Result;

            if (response.IsSuccessStatusCode)
            {
                item = CreateTodoItem(response.Content.ReadAsStringAsync().Result);
            }

            return item;
        }

        static Uri Create(TodoItem item)
        {
            HttpContent content = new StringContent(CreateJson(item), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.Headers.Location; 
            }
            return null;
        }

        static TodoItem Update(TodoItem item)
        {

            TodoItem newItem = null; 
            HttpContent content = new StringContent(CreateJson(item), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PutAsync($"{apiUrl}{item.Id}", content).Result;

            if (response.IsSuccessStatusCode)
            {
                newItem = CreateTodoItem(response.Content.ReadAsStringAsync().Result); 
            }

            return newItem; 
        }

        static HttpStatusCode Delete(long id)
        {
            HttpResponseMessage response = client.DeleteAsync($"{apiUrl}{id}").Result;
            return response.StatusCode; 
        }
    }

    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }

        public string Info()
        {
            return $"Id: {Id}\nName:{Name}\nIdComplete:{IsComplete}";
        }
    }
}
