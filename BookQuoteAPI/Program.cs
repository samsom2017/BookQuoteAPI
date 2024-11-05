using BookQuoteAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL; // Add this for PostgreSQL

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Use PostgreSQL for DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)); // Updated to UseNpgsql for PostgreSQL
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString)); // Updated to UseNpgsql for PostgreSQL

// Add Identity services
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AuthDbContext>();

// Add CORS policy here, before builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.WithOrigins("https://bookquoteapp.onrender.com") // Updated to include Render domain
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
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
app.MapGet("/", () => Results.Redirect("/swagger"));
// Apply CORS policy after app is built
app.UseCors("AllowAllOrigins");

/*
 builder.Services.AddControllers();
var app = builder.Build();
 */

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapIdentityApi<IdentityUser>();

// Set up dynamic port binding for Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://*:{port}");

app.Run();
