using Microsoft.Extensions.Logging;
using WebApp;
using WebApp.Controllers; 

var builder = WebApplication.CreateBuilder(args);

var startup = new StartUp( builder.Configuration );

//var autoresController = new AutoresController( new ApplicationDbContext(null), new ServicioA( new Logger() ));
//autoresController.Get();

startup.ConfigureServices( builder.Services );

var app = builder.Build();

var logger = app.Services.GetService(typeof(ILogger<StartUp>)) as ILogger<StartUp>; 
startup.Configure( app, app.Environment, logger);


app.Run();
