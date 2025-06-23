using PcAnketProject.Data.Repository;
using PcAnketProject.Service;
using PcAnketProject.Data.Context;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DbContext>(provider => new DbContext(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<GirisRepository>();
builder.Services.AddScoped<PusulaAuthService>();
builder.Services.AddScoped<KullaniciRepository>();
builder.Services.AddScoped<KullaniciService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
