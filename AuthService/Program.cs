using Microsoft.EntityFrameworkCore;
using AuthService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddControllers();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

    var retries = 10;
    while (retries > 0)
    {
        try
        {
            Console.WriteLine("Attempting DB migration...");

            // 🔥 Ensure database exists first
            db.Database.EnsureCreated();

            db.Database.Migrate();

            Console.WriteLine("Migration successful!");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            Console.WriteLine($"Database not ready. Retries left: {retries}");
            Thread.Sleep(3000);
        }
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();