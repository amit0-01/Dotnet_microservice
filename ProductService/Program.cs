using Microsoft.EntityFrameworkCore;
using ProductService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔥 PROPER DB RETRY LOGIC
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var retryCount = 10;
    var delay = TimeSpan.FromSeconds(5);

    while (retryCount > 0)
    {
        try
        {
            Console.WriteLine("Attempting DB migration...");
            db.Database.Migrate();
            Console.WriteLine("Database migration successful!");
            break;
        }
        catch (Exception ex)
        {
            retryCount--;
            Console.WriteLine($"Database not ready. Retries left: {retryCount}");
            Console.WriteLine(ex.Message);

            if (retryCount == 0)
                throw;

            Thread.Sleep(delay);
        }
    }
}

app.UseAuthorization();
app.MapControllers();
app.Run();