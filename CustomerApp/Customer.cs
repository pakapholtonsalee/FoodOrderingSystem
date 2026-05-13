using System.Text;
using System.Text.Json;

namespace CustomerApp;

public partial class Customer : Form
{
    Dictionary<string, int> prices = new()
    {
        { "Pizza", 199 },
        { "Burger", 129 },
        { "Steak", 399 }
    };

    int total = 0;

    public Customer()
    {
        InitializeComponent();

        LoadMenu();
    }

    void LoadMenu()
    {
        listBoxMenu.Items.Add("Pizza");
        listBoxMenu.Items.Add("Burger");
        listBoxMenu.Items.Add("Steak");
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        if (listBoxMenu.SelectedItem == null)
            return;

        string item = listBoxMenu.SelectedItem.ToString();

        listBoxCart.Items.Add(item);

        total += prices[item];

        lblTotal.Text = $"Total: {total}";
    }

    private async void btnOrder_Click(object sender, EventArgs e)
    {
        var order = new
        {
            customerName = "Customer",
            restaurantId = 1,
            status = "Pending",
            items = listBoxCart.Items.Cast<string>().ToList(),
            total = total
        };

        var json = JsonSerializer.Serialize(order);

        var content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        HttpClient client = new HttpClient();

        var response = await client.PostAsync(
            "https://localhost:7172/api/orders",
            content);

        if (response.IsSuccessStatusCode)
        {
            MessageBox.Show("Order Success!");

            listBoxCart.Items.Clear();

            total = 0;

            lblTotal.Text = "Total: 0";
        }
        else
        {
            MessageBox.Show("Order Failed");
        }
    }
}