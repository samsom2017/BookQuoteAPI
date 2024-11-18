using BookQuoteAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL; // Add this for PostgreSQL
using Microsoft.IdentityModel.Tokens;  // Required for JWT authentication
using System.Text;  // Add this line for Encoding
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);


// Retrieve the connection string from either appsettings.json or the environment variable
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured. Please check appsettings.json or environment variables.");
}



// Use PostgreSQL for ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Use PostgreSQL for AuthDbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Identity services
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AuthDbContext>();
/*
// Add authentication with JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

*/



var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});


// Add CORS policy here, before builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("https://bookquoteapp.onrender.com")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .SetIsOriginAllowed(origin => true);

    });
});


// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "AuthAPI",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "Please enter a token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Applies any pending migrations or
                                  // creates the database if it doesn't exist
}
app.MapGet("/", () => Results.Redirect("/swagger"));

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

// Apply CORS policy after app is built
app.UseRouting();

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
//add check
app.UseStatusCodePages();
app.UseAuthorization();
app.MapControllers();
app.MapIdentityApi<IdentityUser>();

// Set up dynamic port binding for Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Urls.Add($"http://*:{port}");

app.Run();