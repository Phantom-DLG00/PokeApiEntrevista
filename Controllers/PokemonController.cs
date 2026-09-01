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

    // Guarda el servicio que crea archivos Excel
    private readonly IPokemonExcelExportService _pokemonExcelExportService;
    public PokemonController(
        IPokeApiService pokeApiService,
        IPokemonExcelExportService pokemonExcelExportService,
        ILogger<PokemonController> logger)
    {
        // Guarda el servicio de PokeAPI
        _pokeApiService = pokeApiService;

        // Guarda el servicio de exportacion
        _pokemonExcelExportService =
            pokemonExcelExportService;

        // Guarda el logger
        _logger = logger;
    }

    public async Task<IActionResult> Index(
        string? name,
        string? species,
        int page = 1,
        int pageSize = 24,
        bool export = false,
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

        // Limpia el nombre de la especie recibida
        var speciesFilter = species?.Trim() ?? string.Empty;

        // Calcula desde que Pokemon debe comenzar la consulta
        var offset = checked((page - 1) * pageSize);

        // Crea el ViewModel que recibira la vista
        var viewModel = new PokemonListViewModel
        {
            CurrentPage = page,
            PageSize = pageSize,
            NameFilter = nameFilter,
            SpeciesFilter = speciesFilter
        };

        try
        {

            // Obtiene las especies disponibles para el selector
            var speciesResponse = await _pokeApiService
                .GetSpeciesAsync(cancellationToken);

            // Guarda los nombres de las especies en el ViewModel
            viewModel.Species = speciesResponse.Results
                .Select(species => species.Name)
                .ToList();

            // Declara los Pokemon que se mostraran en la pagina actual
            IEnumerable<PokeApiResource> resourcesForPage;

            if (string.IsNullOrWhiteSpace(nameFilter) &&
                string.IsNullOrWhiteSpace(speciesFilter))
            {
                // Solicita solamente la pagina actual desde PokeAPI
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
                // Obtiene todos los Pokemon para aplicar los filtros
                var response = await _pokeApiService
                    .GetAllPokemonAsync(cancellationToken);

                // Comienza utilizando todos los Pokemon disponibles
                IEnumerable<PokeApiResource> filteredResources =
                    response.Results;

                if (!string.IsNullOrWhiteSpace(speciesFilter))
                {
                    // Obtiene los Pokemon relacionados con la especie seleccionada
                    var speciesDetail = await _pokeApiService
                        .GetSpeciesDetailAsync(
                            speciesFilter,
                            cancellationToken);

                    // Guarda los nombres relacionados con la especie
                    var speciesPokemonNames = speciesDetail.Varieties
                        .Select(variety => variety.Pokemon.Name)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    // Conserva solamente los Pokemon de la especie seleccionada
                    filteredResources = filteredResources
                        .Where(resource =>
                            speciesPokemonNames.Contains(resource.Name));
                }

                if (!string.IsNullOrWhiteSpace(nameFilter))
                {
                    // Conserva los Pokemon que coinciden con el nombre buscado
                    filteredResources = filteredResources
                        .Where(resource =>
                            resource.Name.Contains(
                                nameFilter,
                                StringComparison.OrdinalIgnoreCase));
                }

                // Convierte el resultado filtrado en una lista
                var matchingResources = filteredResources.ToList();

                // Guarda la cantidad total de resultados filtrados
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

            if (export && viewModel.Pokemon.Count > 0)
            {
                // Crea el archivo Excel con los Pokemon visibles
                var fileBytes = _pokemonExcelExportService
                    .CreateExcel(
                        viewModel.Pokemon,
                        viewModel.NameFilter,
                        viewModel.SpeciesFilter,
                        viewModel.CurrentPage,
                        viewModel.PageSize);

                // Define el nombre del archivo descargable
                var fileName =
                    $"pokemon-pagina-{viewModel.CurrentPage}.xlsx";

                // Devuelve el archivo Excel al navegador
                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }

            // Envia los datos preparados a la view
            return View(viewModel);
        }
        catch (OperationCanceledException)
    when (cancellationToken.IsCancellationRequested)
        {
            // Registra que la solicitud fue cancelada por el usuario
            _logger.LogInformation(
                "La solicitud fue cancelada por el usuario");

            // Finaliza la solicitud sin mostrar un error
            return new EmptyResult();
        }
        catch (TaskCanceledException exception)
        {
            // Registra que PokeAPI tardo demasiado tiempo
            _logger.LogWarning(
                exception,
                "La solicitud a PokeAPI supero el tiempo limite");

            // Define el mensaje relacionado con el tiempo de espera
            viewModel.ErrorMessage =
                "La solicitud tardo demasiado tiempo. Intenta nuevamente";

            // Muestra la view con el mensaje de tiempo de espera
            return View(viewModel);
        }
        catch (HttpRequestException exception)
        {
            // Registra el error de comunicacion con PokeAPI
            _logger.LogError(
                exception,
                "No fue posible comunicarse con PokeAPI");

            // Define el mensaje relacionado con la conexion
            viewModel.ErrorMessage =
                "No fue posible conectarse con PokeAPI";

            // Muestra la view con el mensaje de conexion
            return View(viewModel);
        }
        catch (Exception exception)
        {
            // Registra cualquier error que no haya sido contemplado
            _logger.LogError(
                exception,
                "Ocurrio un error inesperado");

            // Define un mensaje general para el usuario
            viewModel.ErrorMessage =
                "Ocurrio un error inesperado";

            // Muestra la view con el mensaje general
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