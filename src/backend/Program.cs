using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Hubs;
using CajuAjuda.Backend.Middlewares;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<GlobalExceptionHandler>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CajuAjudaDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IChamadoRepository, ChamadoRepository>();
builder.Services.AddScoped<IMensagemRepository, MensagemRepository>();
builder.Services.AddScoped<IAnexoRepository, AnexoRepository>();
builder.Services.AddScoped<IRespostaProntaRepository, RespostaProntaRepository>();

// Serviços
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IChamadoService, ChamadoService>();
builder.Services.AddScoped<IMensagemService, MensagemService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IFileStorageService, LocalStorageService>();
builder.Services.AddScoped<IRespostaProntaService, RespostaProntaService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailTemplateService>();
builder.Services.AddScoped<IAIService, AIService>();

builder.Services.AddTransient<DataInitializer>();
builder.Services.AddSignalR();
builder.Services.AddCors();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        // Permitir token via query string para SignalR
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificacaoHub"))
            {
                context.Token = accessToken;
                Console.WriteLine($"[SignalR] Token recebido via query string para {path}");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("--- FALHA NA AUTENTICAÇÃO DO TOKEN JWT ---");
            Console.WriteLine($"Tipo de Falha: {context.Exception.GetType().Name}");
            Console.WriteLine($"Mensagem: {context.Exception.Message}");
            Console.WriteLine("--------------------------------------------------");
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", In = ParameterLocation.Header, Type = SecuritySchemeType.Http, Scheme = "Bearer" });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() } });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataInitializer = scope.ServiceProvider.GetRequiredService<DataInitializer>();
    await dataInitializer.SeedDataAsync();
}

// Habilitar Swagger em todos os ambientes
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalExceptionHandler>();

// CORS configurado para SignalR e Azure
var frontendUrl = builder.Configuration["FRONTEND_URL"] ?? "https://blue-bay-088f9e20f.azurestaticapps.net";
app.UseCors(policy => policy
    .WithOrigins(
        "http://localhost:3000", 
        "http://localhost:3001",
        frontendUrl
    ) 
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .WithExposedHeaders("*")); // Expor todos os headers para SignalR

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificacaoHub>("/notificacaoHub");

app.Run();