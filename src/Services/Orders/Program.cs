using ClickToCart.Orders;
using ClickToCart.Orders.Api;
using ClickToCart.Shared.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults("orders");
builder.Services.AddOrdersModule(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapOrderEndpoints();
app.Run();
