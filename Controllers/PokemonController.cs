using Microsoft.AspNetCore.Mvc;
using PokeApiEntrevista.Models.PokeApi;
using PokeApiEntrevista.Services;
using PokeApiEntrevista.ViewModels;

namespace PokeApiEntrevista.Controllers;

public sealed class PokemonController : Controller
{
    // Guarda el servicio que obtiene informacion desde PokeAPI
    private readonly IPokeApiService _pokeApiService;

    // Permite registrar errores tecnicos de la aplicacion
    private readonly ILogger<PokemonController> _logger;

    public PokemonController(
        IPokeApiService pokeApiService,
        ILogger<PokemonController> logger)
    {
        // Guarda el servicio recibido mediante inyeccion de dependencias
        _pokeApiService = pokeApiService;

        // Guarda el logger recibido mediante inyeccion de dependencias
        _logger = logger;
    }

    public async Task<IActionResult> Index(
        string? name,
        int page = 1,
        int pageSize = 24,
        CancellationToken cancellationToken = default)
    {
        // Evita que el usuario solicite una pagina menor que uno
        if (page < 1)
        {
            page = 1;
        }

        // Define los tamanos de pagina permitidos en la aplicacion
        var allowedPageSizes = new[] { 12, 24, 48, 96 };

        // Utiliza 24 Pokemon si el tamano recibido no esta permitido
        if (!allowedPageSizes.Contains(pageSize))
        {
            pageSize = 24;
        }

        // Limpia los espacios innecesarios del texto recibido
        var nameFilter = name?.Trim() ?? string.Empty;

        // Calcula desde que Pokemon debe comenzar la consulta
        var offset = checked((page - 1) * pageSize);

        // Crea el ViewModel que recibira la vista
        var viewModel = new PokemonListViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            NameFilter = nameFilter
        };

        try
        {
            // Declara los Pokemon que se mostraran en la pagina actual
            IEnumerable<PokeApiResource> resourcesForPage;

            if (string.IsNullOrWhiteSpace(nameFilter))
            {
                // Solicita a PokeAPI los Pokemon de la pagina actual
                var response = await _pokeApiService
                    .GetPokemonPageAsync(
                        pageSize,
                        offset,
                        cancellationToken);

                // Guarda la cantidad total de Pokemon disponibles
                viewModel.TotalCount = response.Count;

                // Utiliza los resultados recibidos para mostrarlos
                resourcesForPage = response.Results;
            }
            else
            {
                // Obtiene todos los Pokemon para aplicar el filtro
                var response = await _pokeApiService
                    .GetAllPokemonAsync(
                        cancellationToken);

                // Conserva los Pokemon que coinciden con la busqueda
                var matchingResources = response.Results
                    .Where(resource =>
                        resource.Name.Contains(
                            nameFilter,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Guarda la cantidad de resultados filtrados
                viewModel.TotalCount = matchingResources.Count;

                // Aplica la paginacion sobre los resultados filtrados
                resourcesForPage = matchingResources
                    .Skip(offset)
                    .Take(pageSize);
            }

            // Calcula cuantas paginas existen en total
            viewModel.TotalPages =
                viewModel.TotalCount == 0
                    ? 0
                    : (int)Math.Ceiling(
                        viewModel.TotalCount / (double)pageSize);

            // Envia al usuario a la ultima pagina si solicito una pagina inexistente
            if (page > viewModel.TotalPages &&
                viewModel.TotalPages > 0)
            {
                return RedirectToAction(
                    nameof(Index),
                    new
                    {
                        page = viewModel.TotalPages,
                        pageSize,
                        name = nameFilter
                    });
            }

            // Convierte los resultados al formato que necesita la view
            viewModel.Pokemon = resourcesForPage
                .Select(MapToViewModel)
                .ToList();

            // Envia los datos preparados a la view
            return View(viewModel);
        }
        catch (Exception exception)
        {
            // Registra el error tecnico para poder revisarlo
            _logger.LogError(
                exception,
                "Ocurrio un error al obtener los Pokemon");

            // Define el mensaje que vera el usuario
            viewModel.ErrorMessage =
                "No fue posible obtener los Pokemon desde PokeAPI";

            // Muestra la view con el mensaje de error
            return View(viewModel);
        }
    }

    private static PokemonListItemViewModel MapToViewModel(
        PokeApiResource resource)
    {
        // Obtiene el numero del Pokemon desde su URL
        var pokemonId = GetPokemonId(resource.Url);

        // Crea el objeto que utilizara la view
        return new PokemonListItemViewModel
        {
            // Guarda el nombre recibido desde PokeAPI
            Name = resource.Name,

            // Construye la URL de la imagen oficial
            ImageUrl =
                $"https://raw.githubusercontent.com/" +
                $"PokeAPI/sprites/master/sprites/pokemon/" +
                $"other/official-artwork/{pokemonId}.png",

            // Guarda la URL original del Pokemon
            ApiUrl = resource.Url
        };
    }

    private static int GetPokemonId(string url)
    {
        // Elimina la diagonal final de la URL
        var cleanUrl = url.TrimEnd('/');

        // Divide la URL en partes utilizando las diagonales
        var parts = cleanUrl.Split('/');

        // Obtiene el ultimo elemento de la URL
        var lastPart = parts[^1];

        // Intenta convertir el ultimo elemento en un numero
        if (int.TryParse(lastPart, out var pokemonId))
        {
            // Devuelve el identificador encontrado
            return pokemonId;
        }

        // Devuelve cero cuando no se pudo obtener un identificador valido
        return 0;
    }
}