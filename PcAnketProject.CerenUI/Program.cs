using Microsoft.AspNetCore.Razor.TagHelpers;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache(); // Gerekli bir cache sağlayıcı
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true; // Sadece HTTP üzerinden erişim
    options.Cookie.IsEssential = true; // GDPR uyumu
});

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500"); // 500 için özel hata sayfası
    app.UseStatusCodePagesWithRedirects("/Error/{0}"); // Tüm hata kodlarını yönlendir
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession(); // Oturum yönetimini etkinleştir
app.UseHsts();

// Middleware: Kullanıcı doğrulama kontrolü
app.Use(async (context, next) =>
{
    var exemptPaths = new[]
    {
        "/", "/Index", "/Home/Index", "/Sorgula", "/Home/Sorgula",
        "/Login/Index", "/Giris", "/KomisyonGiris", "/Inventory/List",
        "/Inventory/Detail", "/Inventory/DefinitionTable",
        "/Komisyon/Index", "/Login/KomisyonGiris", "/Ceren/Index"
    };

    if (exemptPaths.Contains(context.Request.Path.Value, StringComparer.OrdinalIgnoreCase) ||
        context.Request.Path.StartsWithSegments("/lib") ||
        context.Request.Path.StartsWithSegments("/css") ||
        context.Request.Path.StartsWithSegments("/js"))
    {
        await next.Invoke();
        return;
    }

    var kullaniciID = context.Session.GetString("KullaniciID");

    if (string.IsNullOrEmpty(kullaniciID))
    {
        context.Response.Redirect("/Giris");
        return;
    }

    await next.Invoke();
});

// Yönlendirme
app.UseRouting();
app.UseAuthorization();

// ---  GÜNCELLENEN KISIM BURASI ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // ← id? eklendi

// Diğer özel route'lar

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Komisyon",
    pattern: "{controller=Komisyon}/{action=Index}");

app.MapControllerRoute(
    name: "Giris",
    pattern: "{controller=Login}/{action=Giris}");

app.MapControllerRoute(
    name: "Ceren",
    pattern: "{controller=Ceren}/{action=Index}");

app.MapControllerRoute(
    name: "Cikis",
    pattern: "{controller=Login}/{action=Logout}");

app.MapControllerRoute(
    name: "yonetici",
    pattern: "{controller=Komisyon}/{action=Index}");

app.Run();
