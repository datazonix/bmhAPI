using bmhAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static bmhAPI.Data.ApplicationDbContext;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Decide which connection string to use based on environment
var connectionName = builder.Environment.IsDevelopment()
    ? "LocalConnection"
    : "RemoteConnection";

// 2️⃣ Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(connectionName)));

builder.Services.AddScoped<HostelService>();

// 3️⃣ Add Controllers
builder.Services.AddControllers();

// 4️⃣ Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*// 5️⃣ (Optional) Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });
*/
var app = builder.Build();

// 6️⃣ Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // 🔹 Add this if using JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
