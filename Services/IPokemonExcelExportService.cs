using PokeApiEntrevista.ViewModels;

namespace PokeApiEntrevista.Services;

public interface IPokemonExcelExportService
{
    // Crea un archivo Excel con los Pokemon visibles
    byte[] CreateExcel(
        IEnumerable<PokemonListItemViewModel> pokemon,
        string nameFilter,
        string speciesFilter,
        int page,
        int pageSize);
}