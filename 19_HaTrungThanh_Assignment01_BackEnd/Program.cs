using _19_HaTrungThanh_Assignment01_BackEnd.Models;
using _19_HaTrungThanh_Assignment01_BackEnd.Repository;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection string and DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("FUNewsManagementDB")
    ?? "Server=.\\SQLEXPRESS;Database=19_HaTrungThanh_Assignment;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<FUNewsManagementDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Dependency Injection for 3-layer architecture (Repositories call DAOs which use Singleton)
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// 3. Controllers with OData support and Json options
builder.Services.AddControllers()
    .AddOData(opt => opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 5. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
