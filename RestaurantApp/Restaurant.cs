using Microsoft.AspNetCore.SignalR.Client;

namespace RestaurantApp;

public partial class Restaurant : Form
{
    HubConnection connection;

    public Restaurant()
    {
        InitializeComponent();

        InitSignalR();
    }

    async void InitSignalR()
    {
        connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7172/orderHub")
            .Build();

        connection.On<int, string, List<string>, int>(
            "NewOrder",
            (orderId, customer, items, total) =>
            {
                Invoke(() =>
                {
                    string text =
                        $"Order #{orderId}\n" +
                        $"Customer: {customer}\n" +
                        $"Items: {string.Join(", ", items)}\n" +
                        $"Total: {total}\n";

                    listBoxOrders.Items.Add(text);
                });
            });

        await connection.StartAsync();
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }
}