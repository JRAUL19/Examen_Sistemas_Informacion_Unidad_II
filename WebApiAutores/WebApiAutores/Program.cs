using Microsoft.AspNetCore.Identity;
using WebApiAutores;
using WebApiAutores.Entities;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

var servicioLogger = (ILogger<Startup>)app.Services
    .GetService(typeof(ILogger<Startup>));

startup.Configure(app, app.Environment, servicioLogger);

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var loggerFactory = service.GetRequiredService<ILoggerFactory>();

	try
	{
		var userManager = service.GetRequiredService<UserManager<IdentityUser>>();
		var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

		await ApplicationDbContextData.LoadDataAsync(userManager, roleManager, loggerFactory);
    }
	catch (Exception e)
	{
		var logger = loggerFactory.CreateLogger<Program>();
		logger.LogError(e, "Error al inicializar datos");
	}
}

app.Run();
