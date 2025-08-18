using bmhAPI;
using bmhAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Decide which connection string to use based on environment
var connectionName = builder.Environment.IsDevelopment()
    ? "LocalConnection"
    : "RemoteConnection";

// 2️⃣ Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(connectionName)));

// 3️⃣ Register custom services via ConfiguredServices
ConfiguredServices.Register(builder.Services);

// 4️⃣ Add Controllers
builder.Services.AddControllers();

// 5️⃣ Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
// 6️⃣ (Optional) Add JWT Authentication
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

// 7️⃣ Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Uncomment the following line only if JWT Authentication is enabled
// app.UseAuthentication();

app.UseAuthorization();

// 8️⃣ Map Controllers
app.MapControllers();

app.Run();
