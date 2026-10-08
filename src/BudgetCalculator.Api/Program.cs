var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();

// Enables black-box HTTP tests via WebApplicationFactory<Program>.
public partial class Program { }
