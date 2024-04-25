using ByteStorm.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;

// Creamos el constructor de la API
var builder = WebApplication.CreateBuilder(args);
// Especificamos al constructor que al crear JSON se ignoren ciclos y no se escriba algo
// cuando el valor es nulo
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conectamos el contexto de la base de datos con la base de SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BDContext>(option => option.UseSqlite(connectionString));

// Especificamos CORS para que pueda acceder cualquier origen, cabecera y metodo
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod())
);

// Construimos la API
var app = builder.Build();

// Usamos las CORS
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ejecutamos la aplicacion
app.Run();