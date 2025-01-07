using BasketBall_LiveScore.Handlers;
using BasketBall_LiveScore.Hubs;
using BasketBall_LiveScore.Infrastructure;
using BasketBall_LiveScore.Mappers;
using BasketBall_LiveScore.Mappers.Impl;
using BasketBall_LiveScore.Mocks;
using BasketBall_LiveScore.Repositories;
using BasketBall_LiveScore.Repositories.Impl;
using BasketBall_LiveScore.Requirements;
using BasketBall_LiveScore.Services;
using BasketBall_LiveScore.Services.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<LiveScoreContext>(
    options => options
    .UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

// Repositories injection
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchEventRepository, MatchEventRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services injection
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IMatchEventService, MatchEventService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IUserService, UserService>();

// Mappers injection
builder.Services.AddSingleton<IMatchMapper, MatchMapper>();
builder.Services.AddSingleton<IMatchEventMapper, MatchEventMapper>();
builder.Services.AddSingleton<IPlayerMapper, PlayerMapper>();
builder.Services.AddSingleton<ITeamMapper, TeamMapper>();
builder.Services.AddSingleton<IUserMapper, UserMapper>();

builder.Services.AddScoped<IAuthorizationHandler, MatchAssignmentHandler>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Mock Loaders to populate database on startup
builder.Services.AddScoped<MockLoader, PlayersMockLoader>();
builder.Services.AddScoped<MockLoader, TeamsMockLoader>();
builder.Services.AddScoped<MockLoader, UsersMockLoader>();

builder.Services.AddControllers();

// Adding SignalR
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Adding Swagger and JWT integration
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// Adding JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        ClockSkew = TimeSpan.Zero // Allowed clock difference between client and server. Defaults to 5 minutes
    };
});

// Adding JWT authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticationRequired", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EncoderAccess", policy => policy.RequireRole("Encoder", "Admin"));
    options.AddPolicy("MatchAssignmentPolicy", policy => policy.Requirements.Add(new MatchAssignmentRequirement()));
});

builder.Logging
    .ClearProviders()
    .AddConsole()
    .AddDebug();

// Configuring CORS to authorize requests from the front-end
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedClients").Get<string[]>())
              .AllowAnyHeader()
              .AllowCredentials()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

#region DATABASE_POPULATION
// Uncomment this region on first launch to populate the database with starting data data
// /!\ MAKE SURE THIS IS COMMENTED IF YOU ARE NOT TESTING THE APPLICATION IN DEV ENVIRONMENT FOR THE FIRST TIME /!\

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var dbContext = services.GetRequiredService<LiveScoreContext>();
//        if (!dbContext.Database.CanConnect())
//        {
//            await dbContext.Database.EnsureCreatedAsync();

//            var mockLoaders = services.GetServices<MockLoader>();
//            foreach (var loader in mockLoaders)
//            {
//                await loader.PopulateDatabase();
//            }
//        }
//        else if (app.Environment.IsDevelopment())
//        {
//            Console.WriteLine("Dev environment detected. Populating database...");
//            var mockLoaders = services.GetServices<MockLoader>();
//            foreach (var loader in mockLoaders)
//            {
//                await loader.PopulateDatabase();
//            }
//        }
//    } catch (Exception ex)
//    {
//        Console.WriteLine($"Error during database initialization : {ex.Message}");
//    }
//}

#endregion

app.UseCors("AllowAngular");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<MatchHub>("/matchhub");
app.MapHub<PlayerHub>("/playerhub");
app.MapHub<TeamHub>("/teamhub");

app.Run();
