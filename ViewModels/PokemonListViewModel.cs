// Indica el espacio de nombres de este ViewModel.
namespace PokeApiEntrevista.ViewModels;

// Representa toda la información que necesita la vista del listado.
public sealed class PokemonListViewModel
{
    // Cantidad total de Pokémon disponibles en PokeAPI.
    public int TotalCount { get; set; }

    // Lista de Pokémon que mostraremos en pantalla.
    public List<PokemonListItemViewModel> Pokemon { get; set; } =
        new List<PokemonListItemViewModel>();

    // Mensaje que mostraremos si ocurre un error.
    public string? ErrorMessage { get; set; }
}

// Representa un Pokémon individual para mostrarlo en la pantalla.
public sealed class PokemonListItemViewModel
{
    // Nombre del Pokémon.
    public string Name { get; set; } = string.Empty;

    // Dirección de la imagen del Pokémon.
    public string ImageUrl { get; set; } = string.Empty;

    // Dirección del Pokémon dentro de PokeAPI.
    public string ApiUrl { get; set; } = string.Empty;
}