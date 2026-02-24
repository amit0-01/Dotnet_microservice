using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();