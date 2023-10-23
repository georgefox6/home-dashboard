using HomeDashboardApi.Exceptions.Infrastructure;
using HomeDashboardApi.StorageClients;
using MediatR;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("AzureTableStorage");

builder.Services.AddSingleton<TemperatureTableClient>(_ => new TemperatureTableClient(connectionString));
builder.Services.AddSingleton<BinScheduleTableClient>(_ => new BinScheduleTableClient(connectionString));

builder.Services.AddMediatR(typeof(Program).Assembly).AddExceptionHandlers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Home Dashboard",
        Description = "Api for all home dashboard related endpoints.",
        TermsOfService = null,
        Contact = new OpenApiContact
        { Name = "George Fox", Email = "georgefox1996@gmail.com", Url = null }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpContextExceptionHandling();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();