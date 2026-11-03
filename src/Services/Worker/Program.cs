using ClickToCart.Shared.ServiceDefaults;
using ClickToCart.Worker;
using ClickToCart.Worker.Api;
using ClickToCart.Worker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults("worker");
builder.Services.AddWorkerModule();

var app = builder.Build();
app.Services.GetRequiredService<DataSeeder>().Seed();
app.UseServiceDefaults();
app.MapWorkerEndpoints();
app.Run();
