// Indica el espacio de nombres al que pertenece esta clase.
namespace PokeApiEntrevista.Configuration;

// "sealed" indica que esta clase no será heredada por otra clase.
public sealed class PokeApiOptions
{
    // Nombre de la sección que buscaremos dentro de appsettings.json.
    public const string SectionName = "PokeApi";

    // Guarda la dirección base de PokeAPI.
    // "get" permite leer el valor.
    // "set" permite modificar el valor.
    // El valor después de "=" es un valor predeterminado.
    public string BaseUrl { get; set; } =
        "https://pokeapi.co/api/v2/";
}