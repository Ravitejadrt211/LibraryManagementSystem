using LibraryManagementSystem.Models;
using LibraryManagementSystem.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------
// 1️⃣ Configure Services (Dependency Injection)
// ---------------------------------------------------

// ✅ Add MVC Controllers + Views
builder.Services.AddControllersWithViews();

// ✅ Register Database Context (EF Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("con"))
);

// ✅ Register custom services for dependency injection
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<BorrowerService>();
builder.Services.AddScoped<BorrowRecordService>();

// ✅ Enable logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// ---------------------------------------------------
// 2️⃣ Configure Middleware Pipeline
// ---------------------------------------------------

// ✅ Use exception handler for production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅ Enable HTTPS
app.UseHttpsRedirection();

// ✅ Serve static files (CSS, JS, images, etc.)
app.UseStaticFiles();

// ✅ Enable routing
app.UseRouting();

// ✅ Authorization (if you add identity later)
app.UseAuthorization();

// ---------------------------------------------------
// 3️⃣ Configure Default Route
// ---------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Dashboard}/{id?}"
);

// ---------------------------------------------------
// 4️⃣ Run the application
// ---------------------------------------------------
app.Run();
