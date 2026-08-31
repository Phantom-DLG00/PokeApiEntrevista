using PokeApiEntrevista.Models.PokeApi;

namespace PokeApiEntrevista.Services;
public interface IPokeApiService
{
    // Declara un método para obtener una página de Pokémon.

    // "Task" indica que la operación será asíncrona.
    // "PokeApiListResponse" indica el tipo de resultado.
    // "limit" indica cuántos Pokémon solicitaremos.
    // "offset" indica desde qué posición comenzaremos.
    // "CancellationToken" permite cancelar la operación si es necesario.
    Task<PokeApiListResponse> GetPokemonPageAsync(
        int limit,
        int offset,
        CancellationToken cancellationToken = default);

    
    // Obtiene todos los Pokemon para realizar filtros locales
    Task<PokeApiListResponse> GetAllPokemonAsync(CancellationToken cancellationToken = default);

    // Obtiene las especies disponibles en PokeAPI
    Task<PokeApiSpeciesResponse> GetSpeciesAsync(CancellationToken cancellationToken = default);

    // Obtiene el detalle de una especie especifica
    Task<PokeApiSpeciesDetailResponse> GetSpeciesDetailAsync(string speciesName, CancellationToken cancellationToken = default);
    
}