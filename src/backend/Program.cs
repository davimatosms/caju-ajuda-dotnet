// 1. Todas as diretivas 'using' devem vir primeiro, no topo do arquivo.
using Microsoft.EntityFrameworkCore;
using CajuAjuda.Backend.Data;
using CajuAjuda.Backend.Repositories;
using CajuAjuda.Backend.Services;

// 2. A variável 'builder' é criada apenas UMA vez.
var builder = WebApplication.CreateBuilder(args);

// 3. Aqui adicionamos todos os serviços que a aplicação vai usar.
// Pega a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o DbContext e configura para usar o SQL Server
builder.Services.AddDbContext<CajuAjudaDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();


// Serviços para a documentação da API (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddControllers();


// 4. A variável 'app' é criada para configurar o pipeline de requisições.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapeia os controllers que criaremos no futuro
app.MapControllers();


app.Run();