// Importa el modelo que utilizaremos como respuesta.
using PokeApiEntrevista.Models.PokeApi;

// Indica el espacio de nombres del servicio.
namespace PokeApiEntrevista.Services;

// La letra "I" al inicio indica que es una interfaz.
// Una interfaz define qué acciones debe ofrecer una clase.
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
}