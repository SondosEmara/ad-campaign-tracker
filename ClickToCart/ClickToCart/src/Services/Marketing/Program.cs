using ClickToCart.Marketing;
using ClickToCart.Marketing.Api;
using ClickToCart.Shared.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults("marketing");
builder.Services.AddMarketingModule(builder.Configuration);

var app = builder.Build();
app.UseServiceDefaults();
app.MapCampaignEndpoints();
app.MapLinkEndpoints();
app.MapReportingEndpoints();
app.MapRedirectEndpoints();
app.Run();
