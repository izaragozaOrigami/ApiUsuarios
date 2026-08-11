using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Origami.Identity.Api.Core.Data;
using Origami.Identity.Api.Core.Infrastructure;
using Origami.Identity.Api.Core.Services;


//using Origami.Identity.Api.Infrastructure; // Tu ApplicationUser
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Consolidación de blobs (dev): inicializa el resolver con la config (Storage:ConsolidatedContainer/BlobBaseUri).
Origami.Identity.Api.Core.Infrastructure.UserStorageResolver.Init(builder.Configuration);

// ============================
// 1?? Configurar DB y Identity
// ============================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ============================
// 2?? Configurar JWT Authentication
// ============================

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//Activar CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
                 // .AllowCredentials(); // si usas cookies o auth
        });
});


// ============================
// 3?? Agregar controladores
// ============================

builder.Services.AddControllers();

// ============================
// 4?? Configurar Swagger
// ============================

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<DataHelper>();
builder.Services.AddScoped<SecurityData>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Origami Identity API Core",
        Version = "v1"
    });

    // JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer {token}'"
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
            new string[]{}
        }
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");
// ============================
// 5?? Middleware
// ============================

// Swagger UI en la ra�z
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Origami Identity API Core v1");
    c.RoutePrefix = ""; // Swagger directamente en https://localhost:7232/
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

app.UseDeveloperExceptionPage();


