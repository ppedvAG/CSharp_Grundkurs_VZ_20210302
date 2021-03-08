using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Lab16_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        string apiUrl = "api/todoitems/";
        public MainWindow()
        {
            InitializeComponent();

            client.BaseAddress = new Uri("https://meineurl/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async void btnSuche_Click(object sender, RoutedEventArgs e)
        {
            string suche = tbSuche.Text;
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}{suche}");

            if (response.IsSuccessStatusCode)
            {
                List<TodoItem> items = null;
                if (suche.Contains("-"))
                {
                    items = CreateTodoItems(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    TodoItem item = CreateTodoItem(await response.Content.ReadAsStringAsync());
                    await UpdateUI(item);
                    items = new List<TodoItem>() { item };
                }
                dgItems.ItemsSource = items; 

            }
            else
            {
                MessageBox.Show("Nichts gefunden..");
            }
        }

        private async void btnBearbeiten_Click(object sender, RoutedEventArgs e)
        {
            TodoItem item = new TodoItem();
            item.Id = Convert.ToInt64(tbId.Text);
            item.Name = tbName.Text;
            item.IsComplete = cbErledigt.IsChecked ?? true;

            HttpContent content = new StringContent(CreateJson(item), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"{apiUrl}{item.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TodoItem newItem = CreateTodoItem(await response.Content.ReadAsStringAsync());
                await UpdateUI(newItem);
                dgItems.ItemsSource = new List<TodoItem>() { newItem };
            }
            else
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten! Status:{response.StatusCode}");
            }
        }

        private async void btnNeu_Click(object sender, RoutedEventArgs e)
        {
            TodoItem item = new TodoItem();
            item.Name = tbName.Text;
            item.IsComplete = cbErledigt.IsChecked ?? false;

            HttpContent content = new StringContent(CreateJson(item), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TodoItem newItem = CreateTodoItem(await response.Content.ReadAsStringAsync());
                await UpdateUI(newItem);
                dgItems.ItemsSource = new List<TodoItem>() { newItem };
            }
            else
            {
                MessageBox.Show($"Es ist ein Fehler aufgetreten! Status:{response.StatusCode}");
            }
        }

        private async void btnEntfernen_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Soll das Item gelöscht werden?", "Löschen", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                long id = Convert.ToInt64(tbId.Text);
                HttpResponseMessage response = await client.DeleteAsync($"{apiUrl}{id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Item wurde erfolgreich gelöscht!");
                }
                else
                {
                    MessageBox.Show($"Es ist ein Fehler aufgetreten! Status:{response.StatusCode}");
                }
            }
        }
        private async Task UpdateUI(TodoItem item)
        {
            tbId.Text = item?.Id.ToString();
            tbName.Text = item?.Name;
            cbErledigt.IsChecked = item?.IsComplete;
        }

        static string CreateJson(TodoItem item)
        {
            return JsonConvert.SerializeObject(item);
        }

        static TodoItem CreateTodoItem(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<TodoItem>(json);
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
                return JsonConvert.DeserializeObject<List<TodoItem>>(json);
            }
            catch
            {
                return null;
            }
        }
    }
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
