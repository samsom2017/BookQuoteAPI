using BookQuoteAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Configuration: Database Connection
ConfigureDatabase(builder);

// Configuration: Identity
ConfigureIdentity(builder);

// Configuration: Authentication (JWT)
ConfigureAuthentication(builder);

// Configuration: CORS
ConfigureCors(builder);

// Add Controllers and API Documentation (Swagger)
ConfigureControllersAndSwagger(builder);

var app = builder.Build();

// Apply database migrations on startup
ApplyDatabaseMigrations(app);

// Configure the HTTP request pipeline
ConfigureMiddleware(app);

// Set up dynamic port binding for deployment
SetupDynamicPortBinding(app);

app.Run();

#region Configuration Methods
void ConfigureDatabase(WebApplicationBuilder builder)
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                             ?? Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Connection string is not configured. Please check appsettings.json or environment variables.");
    }

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}

void ConfigureIdentity(WebApplicationBuilder builder)
{
    builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
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
}

void ConfigureCors(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.WithOrigins("https://bookquoteapp.onrender.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .SetIsOriginAllowed(origin => true);
        });
    });
}

void ConfigureControllersAndSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "AuthAPI",
            Version = "v1"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a token",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
}

void ApplyDatabaseMigrations(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

void ConfigureMiddleware(WebApplication app)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

void SetupDynamicPortBinding(WebApplication app)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
    app.Urls.Add($"http://*:{port}");
}
#endregion
