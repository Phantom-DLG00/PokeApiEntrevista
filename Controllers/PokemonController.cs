using Microsoft.AspNetCore.Mvc;
// Permite utilizar el servicio de PokeAPI.
using PokeApiEntrevista.Services;
// Permite utilizar nuestros ViewModels.
using PokeApiEntrevista.ViewModels;
// Indica el espacio de nombres del controlador.
namespace PokeApiEntrevista.Controllers;

// Controller permite recibir solicitudes del navegador.
public sealed class PokemonController : Controller
{
    // Variable que representa el servicio de PokeAPI.
    private readonly IPokeApiService _pokeApiService;

    // Permite registrar errores e información.
    private readonly ILogger<PokemonController> _logger;

    // Constructor del controlador.
    // .NET proporcionará automáticamente el servicio y el logger.
    public PokemonController(
        IPokeApiService pokeApiService,
        ILogger<PokemonController> logger)
    {
        // Guarda el servicio recibido.
        _pokeApiService = pokeApiService;

        // Guarda el logger recibido.
        _logger = logger;
    }

    // Acción que se ejecutará cuando visitemos la ruta /Pokemon.
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        // Creamos un ViewModel vacío.
        var viewModel = new PokemonListViewModel();

        try
        {
            // Cantidad de Pokémon que solicitaremos a PokeAPI.
            const int limit = 24;

            // Posición inicial desde donde comenzaremos a consultar.
            const int offset = 0;

            // Consultamos PokeAPI utilizando nuestro servicio.
            var response = await _pokeApiService
                .GetPokemonPageAsync(
                    limit,
                    offset,
                    cancellationToken);

            // Total de Pokémon.
            viewModel.TotalCount = response.Count;

            // Convertimos los resultados de PokeAPI al formato de la view
            viewModel.Pokemon = response.Results
                .Select(resource =>
                {
                    // Extraemos el número del Pokémon desde su URL.
                    var pokemonId = GetPokemonId(resource.Url);

                    // Creamos un objeto para la vista.
                    return new PokemonListItemViewModel
                    {
                        // Guardamos el nombre.
                        Name = resource.Name,

                        // Construimos la URL de la imagen oficial.
                        ImageUrl =
                            $"https://raw.githubusercontent.com/" +
                            $"PokeAPI/sprites/master/sprites/pokemon/" +
                            $"other/official-artwork/{pokemonId}.png",

                        // Guardamos la URL original de PokeAPI.
                        ApiUrl = resource.Url
                    };
                })
                .ToList();

            // Enviamos el ViewModel a la vista.
            return View(viewModel);
        }
        catch (Exception exception)
        {
            // Registramos el error técnico para poder revisarlo.
            _logger.LogError(
                exception,
                "Ocurrió un error al obtener los Pokémon.");
            viewModel.ErrorMessage =
                "No fue posible obtener los Pokémon desde PokeAPI.";

            // Mostramos la vista con el mensaje de error.
            return View(viewModel);
        }
    }

    // Obtiene el número del Pokémon desde su URL.
    private static int GetPokemonId(string url)
    {
        // Quitamos la diagonal final de la URL.
        var cleanUrl = url.TrimEnd('/');

        // Separamos la URL utilizando cada diagonal.
        var parts = cleanUrl.Split('/');

        // Obtenemos el último elemento.
        var lastPart = parts[^1];

        // Intentamos convertirlo en número.
        if (int.TryParse(lastPart, out var pokemonId))
        {
            // Devolvemos el número encontrado.
            return pokemonId;
        }

        // Devolvemos cero si no se pudo obtener el número.
        return 0;
    }
}