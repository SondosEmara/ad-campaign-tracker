using ClickToCart.Products;
using ClickToCart.Products.Api;
using ClickToCart.Shared.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults("products");
builder.Services.AddProductsModule();

var app = builder.Build();
app.UseServiceDefaults();
app.MapProductEndpoints();
app.Run();
