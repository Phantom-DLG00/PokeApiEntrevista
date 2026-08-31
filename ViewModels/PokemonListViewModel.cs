namespace PokeApiEntrevista.ViewModels;

public sealed class PokemonListViewModel
{
    // Guarda la cantidad total de Pokemon disponibles
    public int TotalCount { get; set; }

    // Guarda los Pokemon que se mostraran en la pagina actual
    public List<PokemonListItemViewModel> Pokemon { get; set; } =
        new List<PokemonListItemViewModel>();

    // Guarda el mensaje que se mostrara cuando ocurra un error
    public string? ErrorMessage { get; set; }

    // Guarda el texto que el usuario escribio en el buscador
    public string NameFilter { get; set; } = string.Empty;

    // Guarda el numero de la pagina que se esta mostrando
    public int CurrentPage { get; set; }

    // Guarda cuantos Pokemon se muestran por pagina
    public int PageSize { get; set; }

    // Calcula cuantas paginas existen en total
    public int TotalPages { get; set; }

    // Indica si existe una pagina anterior
    public bool HasPreviousPage =>
        CurrentPage > 1;

    // Indica si existe una pagina siguiente
    public bool HasNextPage =>
        CurrentPage < TotalPages;
}

public sealed class PokemonListItemViewModel
{
    // Guarda el nombre del Pokemon
    public string Name { get; set; } = string.Empty;

    // Guarda la direccion de la imagen del Pokemon
    public string ImageUrl { get; set; } = string.Empty;

    // Guarda la direccion del Pokemon en PokeAPI
    public string ApiUrl { get; set; } = string.Empty;
}