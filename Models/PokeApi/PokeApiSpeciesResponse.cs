namespace PokeApiEntrevista.Models.PokeApi;

public sealed class PokeApiSpeciesResponse
{
    // Guarda la cantidad total de especies disponibles
    public int Count { get; set; }

    // Guarda los resultados de especies
    public List<PokeApiResource> Results { get; set; } = new List<PokeApiResource>();
}

public sealed class PokeApiSpeciesDetailResponse
{
    // Guarda las diferentes variantes relacionadas con la especie
    public List<PokeApiSpeciesVariety> Varieties { get; set; } =
        new List<PokeApiSpeciesVariety>();
}

public sealed class PokeApiSpeciesVariety
{
    // Guarda la informacion del Pokemon relacionado
    public PokeApiResource Pokemon { get; set; } =
        new PokeApiResource();
}