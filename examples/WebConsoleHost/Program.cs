using System.Diagnostics.CodeAnalysis;
using MATSys.Hosting;

#region For Webapplicatio usage

var builder = WebApplication.CreateBuilder(args);

var startup=new Startup(builder.Configuration);

startup.ConfigureBuilder(builder);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app,builder.Environment);

app.Run();

#endregion
