using GamesAPI.Api.Data;
using Microsoft.EntityFrameworkCore;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using GamesAPI.Api.Middleware;
using GamesAPI.Api.Hubs;
using GamesAPI.Api.Services.Security;

var builder = WebApplication.CreateBuilder(args);
//For Reading JWT settings from appsettings.json
var jwtSettings =
    builder.Configuration
        .GetSection("Jwt");


// Services
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description =
                "Enter JWT token",

            In =
                ParameterLocation.Header,

            Type =
                SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat =
                "JWT"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Id =
                                "Bearer",

                            Type =
                                ReferenceType
                                    .SecurityScheme
                        }
                },

                new string[] { }
            }
        });
});

builder.Services.AddScoped<
    IGameService,
    GameService
    >();
builder.Services.AddScoped<
ITournamentService,
TournamentService
>();
builder.Services.AddScoped<
    ITournamentStatusService,
    TournamentStatusService
    >();
builder.Services.AddScoped<
IUserService,
UserService
>();
builder.Services
    .AddScoped<
        ITournamentMatchesService,
        TournamentMatchesService>();
builder.Services.AddScoped<
    ILogService,
    LogService>();
builder.Services.AddScoped<
INotificationService,
NotificationService>();
builder.Services.AddScoped<
IAdminDashboardService,
AdminDashboardService>();
builder.Services.AddScoped<
IAnnouncementService,
AnnouncementService>();
builder.Services.AddScoped<IOnlineUserService, OnlineUserService>();
builder.Services.AddSingleton<IOnlineUserTracker, OnlineUserTracker>();
builder.Services.AddScoped<
    IConversationService,
    ConversationService>();


//Encrypt Decrypt 
builder.Services.AddSingleton<ICryptoService, CryptoService>();
// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = jwtSettings["Issuer"],
//             ValidAudience = jwtSettings["Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
//     jwtSettings["Key"]!))
//         };
//     });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/announcements"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    }); builder.Services
.AddAuthorization();
//Configure CORS to allow requests from the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowReactApp",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173", "http://localhost:5174")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS
app.UseHttpsRedirection();

// Static files for serving images, etc.
app.UseStaticFiles();

// CORS
app.UseCors("AllowReactApp");
app.UseMiddleware<
    ExceptionMiddleware>();
// JWT middleware
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

app.MapHub<AnnouncementHub>(
    "/hubs/announcements");
app.MapHub<ChatHub>(
"/hubs/chat");
app.Run();