using LocalDBWebAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add CORS policy to allow communication with the web app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:5136") // Adjust this based on the web app's URL
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

DBManager.DBInitialise();

var app = builder.Build();

// Apply the CORS policy
app.UseCors("AllowWebApp");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session support

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
