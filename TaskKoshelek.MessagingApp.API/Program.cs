using Microsoft.OpenApi.Models;
using System.Reflection;
using TaskKoshelek.MessagingApp.API.Extensions;
using TaskKoshelek.MessagingApp.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
builder.Configuration.AddEnvironmentVariables();
if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration["DB_CONNECTION_STRING"];
    Console.WriteLine($"Connection string found: {!string.IsNullOrEmpty(connectionString)}");
}
builder.Services.AddDALServices(builder.Configuration);
builder.Services.AddAPIServices();
builder.Services.AddControllers();
builder.Services.AddOpenTelemetryServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
    Title = "TaskKoshelek.MessagingApp.API",
    Version = "v1",
    Description = "API for managing messages in the system"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseTelemetryMiddleware();
app.UseAuthorization();

app.MapControllers();

app.Run();
