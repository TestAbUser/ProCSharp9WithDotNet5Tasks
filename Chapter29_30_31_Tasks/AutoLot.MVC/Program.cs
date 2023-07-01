using AutoLot.Services.Logging;
using AutoLot.MVC;

var builder = CreateHostBuilder(args);

var app = builder.Build();

app.Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    }).ConfigureSerilog();
