// 1. Usings devem vir primeiro, no topo do arquivo.
using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

// 2. A variável 'builder' é criada apenas UMA vez.
var builder = WebApplication.CreateBuilder(args);

// 3. Aqui adicionamos todos os serviços que a aplicação vai usar (Injeção de Dependência).

// Pega a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o DbContext e configura para usar o SQL Server
builder.Services.AddDbContext<CajuAjudaDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registra nossas interfaces e suas implementações
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IChamadoRepository, ChamadoRepository>();
builder.Services.AddScoped<IChamadoService, ChamadoService>();
builder.Services.AddScoped<IMensagemRepository, MensagemRepository>();
builder.Services.AddScoped<IMensagemService, MensagemService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Registra o DataInitializer para popular o banco de dados
builder.Services.AddTransient<DataInitializer>();

// Configura a autenticação com JWT Bearer
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Adiciona o serviço de controllers para a API
builder.Services.AddControllers();

// Adiciona os serviços para a documentação da API (Swagger) com suporte para JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Adiciona a definição de segurança para "Bearer" (JWT)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    // Adiciona o requisito de segurança que usa a definição "Bearer"
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
            Array.Empty<string>()
        }
    });
});


// 4. A variável 'app' é criada para configurar o pipeline de requisições.
var app = builder.Build();

// Executa o DataInitializer para popular o banco de dados na inicialização
using (var scope = app.Services.CreateScope())
{
    var dataInitializer = scope.ServiceProvider.GetRequiredService<DataInitializer>();
    await dataInitializer.SeedDataAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Adiciona os middlewares de Autenticação e Autorização (A ORDEM É IMPORTANTE)
app.UseAuthentication(); // 1. Verifica quem é o usuário (valida o token)
app.UseAuthorization();  // 2. Verifica se o usuário tem permissão para acessar

// Mapeia os controllers que criamos (como AuthController, ChamadosController, etc.)
app.MapControllers();

// 5. Inicia a aplicação.
app.Run();