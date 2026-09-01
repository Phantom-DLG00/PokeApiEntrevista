using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
// Importa la clase de configuración de PokeAPI.
using PokeApiEntrevista.Configuration;
// Importa la interfaz y la implementación del servicio.
using PokeApiEntrevista.Services;

***REMOVED***

// Crea el constructor principal de la aplicación.
var builder = WebApplication.CreateBuilder(args);

// Registra MVC con controladores y vistas.
builder.Services.AddControllersWithViews();

// Lee la sección "PokeApi" desde appsettings.json.
builder.Services.Configure<PokeApiOptions>(
    builder.Configuration.GetSection(
        PokeApiOptions.SectionName));

// Registra HttpClient y el servicio de PokeAPI.
builder.Services.AddHttpClient<
    IPokeApiService,
    PokeApiService>(
    (serviceProvider, client) =>
    {
        // Obtiene la configuración de PokeAPI.
        var options = serviceProvider
            .GetRequiredService<
                IOptions<PokeApiOptions>>()
            .Value;

        // Define la dirección base de PokeAPI.
        client.BaseAddress = new Uri(options.BaseUrl);

        // Define un tiempo máximo de espera de 10 segundos.
        client.Timeout = TimeSpan.FromSeconds(10);

        // Indica que esperamos recibir JSON.
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"));
    });
// Registra el servicio encargado de crear archivos Excel
builder.Services.AddScoped<IPokemonExcelExportService,PokemonExcelExportService>();

// Lee la configuracion del correo desde appsettings y User Secrets
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));

// Registra el servicio de envio de correos
builder.Services.AddScoped<
    IEmailService,
    SmtpEmailService>();
    
// Construye la aplicación.
var app = builder.Build();
// Si la aplicación no está en modo desarrollo...
if (!app.Environment.IsDevelopment())
{
    // Muestra una página general cuando ocurre un error.
    app.UseExceptionHandler("/Home/Error");

    // Activa reglas de seguridad para HTTPS.
    app.UseHsts();
}

// Redirige las solicitudes HTTP hacia HTTPS.
app.UseHttpsRedirection();
// Permite utilizar archivos CSS, JavaScript e imágenes.
app.UseStaticFiles();
// Activa el sistema de rutas.
app.UseRouting();
// Activa la autorización.
app.UseAuthorization();

// Define la ruta principal de MVC.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();