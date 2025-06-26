using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using BackendApi.Data;
using BackendApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing from configuration.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing from configuration.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Swagger + JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// ==== SEED ROLES AND DEFAULT ADMIN ====
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Seed roles
    string[] roles = { "Admin", "Teacher", "Student", "Parent" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Custom admin seed logic
    async Task SeedAdminUserAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        string adminEmail = "admin@school.com";
        string password = "Admin@123";

        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser == null)
        {
            var adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");

                var adminEntity = new Admin
                {
                    AdminId = Guid.NewGuid().ToString(),
                    UserId = adminUser.Id,
                    Department = "System"
                };

                context.Admins.Add(adminEntity);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Failed to seed admin user:");
                foreach (var error in result.Errors)
                    Console.WriteLine($" - {error.Description}");
            }
        }
    }

    await SeedAdminUserAsync(userManager, roleManager, dbContext);
}

// Middlewares
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
