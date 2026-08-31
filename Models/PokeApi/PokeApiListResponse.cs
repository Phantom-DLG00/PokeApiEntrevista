// Indica el espacio de nombres de estos modelos.
namespace PokeApiEntrevista.Models.PokeApi;

// Esta clase representa la respuesta general de PokeAPI.
public sealed class PokeApiListResponse
{
    // Cantidad total de Pokémon disponibles en PokeAPI.
    public int Count { get; set; }

    // URL para obtener la página siguiente.
    // El signo "?" indica que el valor puede ser null.
    public string? Next { get; set; }

    // URL para obtener la página anterior.
    // Puede ser null cuando estamos en la primera página.
    public string? Previous { get; set; }

    // Lista de Pokémon devueltos por la API.
    // Se inicializa como una lista vacía para evitar valores null.
    public List<PokeApiResource> Results { get; set; } =
        new List<PokeApiResource>();
}

// Esta clase representa un recurso individual de PokeAPI.
public sealed class PokeApiResource
{
    // Nombre del Pokémon.
    public string Name { get; set; } = string.Empty;

    // URL del recurso individual del Pokémon.
    public string Url { get; set; } = string.Empty;
}