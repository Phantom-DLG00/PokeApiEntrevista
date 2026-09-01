using PokeApiEntrevista.ViewModels;

namespace PokeApiEntrevista.Services;

public interface IEmailService
{
    // Envia la informacion de un Pokemon por correo
    Task SendPokemonAsync(
        string recipientEmail,
        PokemonListItemViewModel pokemon,
        CancellationToken cancellationToken = default);
}