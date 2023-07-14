using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using AutoLot.Api;
using AutoLot.Services.Logging;


var builder = CreateHostBuilder(args);

var app = builder.Build();

app.Run();

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    }).ConfigureSerilog();






