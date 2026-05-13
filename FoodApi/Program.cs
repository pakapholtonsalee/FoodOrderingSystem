using FoodApi.Data;
using FoodApi.Hubs;
using FoodApi.Repositories;
using FoodApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<FoodContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// SignalR (real-time)
builder.Services.AddSignalR();

// Repository + Service (Dependency Injection)
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// --- App ---
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/orderHub");

app.Run();