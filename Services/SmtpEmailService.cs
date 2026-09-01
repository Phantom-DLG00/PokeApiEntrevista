using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using PokeApiEntrevista.Configuration;
using PokeApiEntrevista.ViewModels;

namespace PokeApiEntrevista.Services;

public sealed class SmtpEmailService : IEmailService
{
    // Guarda la configuracion del servidor SMTP
    private readonly EmailOptions _emailOptions;

    public SmtpEmailService(
        IOptions<EmailOptions> emailOptions)
    {
        // Guarda la configuracion recibida
        _emailOptions = emailOptions.Value;
    }

    public async Task SendPokemonAsync(
        string recipientEmail,
        PokemonListItemViewModel pokemon,
        CancellationToken cancellationToken = default)
    {
        // Crea el mensaje que sera enviado
        using var message = new MailMessage();

        // Define la direccion del remitente
        message.From = new MailAddress(
            _emailOptions.FromAddress,
            _emailOptions.FromName);

        // Define la direccion del destinatario
        message.To.Add(recipientEmail);

        // Define el asunto del correo
        message.Subject =
            $"Informacion del Pokemon {pokemon.Name}";

        // Permite utilizar etiquetas HTML en el mensaje
        message.IsBodyHtml = true;

        // Construye el contenido del correo
        message.Body = $"""
            <h1>{pokemon.Name}</h1>
            <p>Informacion obtenida desde PokeAPI</p>
            <p>
                <strong>Nombre:</strong>
                {pokemon.Name}
            </p>
            <p>
                <strong>URL de imagen:</strong>
                <a href="{pokemon.ImageUrl}">
                    Ver imagen
                </a>
            </p>
            <p>
                <strong>URL de PokeAPI:</strong>
                <a href="{pokemon.ApiUrl}">
                    Ver informacion
                </a>
            </p>
            """;

        // Crea el cliente que se conectara con el servidor SMTP
        using var smtpClient = new SmtpClient(
            _emailOptions.Host,
            _emailOptions.Port);

        // Configura la seguridad de la conexion
        smtpClient.EnableSsl = _emailOptions.EnableSsl;

        // Configura las credenciales del servidor SMTP
        smtpClient.Credentials = new NetworkCredential(
            _emailOptions.UserName,
            _emailOptions.Password);

        // Envia el correo sin bloquear la aplicacion
        await smtpClient.SendMailAsync(
            message,
            cancellationToken);
    }
}