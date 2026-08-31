using System.Net.Http.Json;
// Importa los modelos de respuesta de PokeAPI.
using PokeApiEntrevista.Models.PokeApi;
// Indica el espacio de nombres del servicio.
namespace PokeApiEntrevista.Services;

// Esta clase implementa la interfaz IPokeApiService.
public sealed class PokeApiService : IPokeApiService
{
    // HttpClient será utilizado para comunicarnos con PokeAPI.
    private readonly HttpClient _httpClient;

    // ILogger permite escribir información y errores en los registros.
    private readonly ILogger<PokeApiService> _logger;

    // Constructor de la clase.
    // Recibe HttpClient y ILogger automáticamente.
    public PokeApiService(
        HttpClient httpClient,
        ILogger<PokeApiService> logger)
    {
        // Guarda el HttpClient recibido en la variable privada.
        _httpClient = httpClient;
        // Guarda el logger recibido en la variable privada.
        _logger = logger;
    }

    // Método que obtiene una página de Pokémon.
    // "async" indica que contiene una operación asíncrona.
    public async Task<PokeApiListResponse> GetPokemonPageAsync(
        int limit,
        int offset,
        CancellationToken cancellationToken = default)
    {
        // Verifica que la cantidad solicitada sea mayor que cero.
        if (limit <= 0)
        {
            // Detiene la ejecución si el valor no es válido.
            throw new ArgumentOutOfRangeException(nameof(limit));
        }

        // Verifica que offset no sea negativo.
        if (offset < 0)
        {
            // Detiene la ejecución si el valor no es válido.
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        // Construye la dirección relativa que consultaremos.
        // Por ejemplo: pokemon?limit=20&offset=0
        var endpoint = $"pokemon?limit={limit}&offset={offset}";

        // Envía una solicitud GET a PokeAPI.
        // "await" espera la respuesta sin bloquear la aplicación.
        // "using" libera la respuesta después de utilizarla.
        using var response = await _httpClient.GetAsync(
            endpoint,
            cancellationToken);

        // Comprueba si PokeAPI respondió correctamente.
        if (!response.IsSuccessStatusCode)
        {
            // Escribe el código de error en los registros.
            _logger.LogError(
                "PokeAPI respondió con el código {StatusCode}.",
                response.StatusCode);

            // Genera un error controlado.
            throw new HttpRequestException(
                $"PokeAPI respondió con {response.StatusCode}.");
        }

        // Lee el contenido JSON y lo convierte en PokeApiListResponse.
        var result = await response.Content
            .ReadFromJsonAsync<PokeApiListResponse>(
                cancellationToken);

        // Verifica que la respuesta no esté vacía.
        if (result is null)
        {
            // Genera un error si PokeAPI no devolvió datos.
            throw new InvalidOperationException(
                "PokeAPI devolvió una respuesta vacía.");
        }

        ***REMOVED***

        // Devuelve los datos convertidos a objetos C#.
        return result;
    }


    // Obtiene todos los Pokemon disponibles en PokeAPI
    public async Task<PokeApiListResponse> GetAllPokemonAsync(CancellationToken cancellationToken = default)
    {
        // Obtiene la cantidad total de Pokemon disponibles
        var firstResponse = await GetPokemonPageAsync(
            1,
            0,
            cancellationToken);

        // Devuelve una respuesta vacia cuando no existen Pokemon
        if (firstResponse.Count == 0)
        {
            return firstResponse;
        }

        // Solicita todos los Pokemon usando la cantidad total encontrada
        return await GetPokemonPageAsync(firstResponse.Count,0,cancellationToken);
    }
}