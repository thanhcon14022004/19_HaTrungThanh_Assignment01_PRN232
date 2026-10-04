using _19_HaTrungThanh_Assignment01_FrontEnd.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services to the container.
builder.Services.AddControllersWithViews();

// Configure Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure HttpClient for BackEnd Web API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5134/api/";
builder.Services.AddHttpClient("BackendApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Register ApiClient
builder.Services.AddScoped<ApiClient>();

// Register HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
