
using Microsoft.OpenApi.Models;

//load up the config from env and appsettings
Viper.Config();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "0.0.1",
        Title = "Tailwind Traders Mail Services API",
        Description = "Transactional and bulk email sending services for Tailwind Traders.",
        Contact = new OpenApiContact
        {
            Name = "Rob Conery, Aaron Wislang, and the Tailwind Traders Team",
            Url = new Uri("https://tailwindtraders.dev")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

Tailwind.Mail.Api.Public.MapRoutes(app);
Tailwind.Mail.Api.Admin.MapRoutes(app);

app.Run();

//this is for tests
public partial class Program { }