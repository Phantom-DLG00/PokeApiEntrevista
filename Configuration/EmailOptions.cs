namespace PokeApiEntrevista.Configuration;

public sealed class EmailOptions
{
    // Guarda la direccion del servidor SMTP
    public string Host { get; set; } = string.Empty;

    // Guarda el puerto del servidor SMTP
    public int Port { get; set; }

    // Indica si la conexion utilizara SSL
    public bool EnableSsl { get; set; }

    // Guarda el usuario del servidor SMTP
    public string UserName { get; set; } = string.Empty;

    // Guarda la contrasena del servidor SMTP
    public string Password { get; set; } = string.Empty;

    // Guarda la direccion que aparecera como remitente
    public string FromAddress { get; set; } = string.Empty;

    // Guarda el nombre que aparecera como remitente
    public string FromName { get; set; } = string.Empty;
}