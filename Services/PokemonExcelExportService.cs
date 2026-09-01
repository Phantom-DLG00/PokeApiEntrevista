using ClosedXML.Excel;
using PokeApiEntrevista.ViewModels;

namespace PokeApiEntrevista.Services;

public sealed class PokemonExcelExportService
    : IPokemonExcelExportService
{
    public byte[] CreateExcel(
        IEnumerable<PokemonListItemViewModel> pokemon,
        string nameFilter,
        string speciesFilter,
        int page,
        int pageSize)
    {
        // Crea un nuevo libro de Excel
        using var workbook = new XLWorkbook();

        // Crea una hoja llamada Pokemon
        var worksheet = workbook.Worksheets.Add("Pokemon");

        // Agrega los encabezados de las columnas
        worksheet.Cell(1, 1).Value = "Nombre";
        worksheet.Cell(1, 2).Value = "URL de imagen";
        worksheet.Cell(1, 3).Value = "URL de PokeAPI";

        // Agrega informacion sobre la consulta realizada
        worksheet.Cell(1, 5).Value = "Filtro por nombre";
        worksheet.Cell(1, 6).Value = nameFilter;

        worksheet.Cell(2, 5).Value = "Filtro por especie";
        worksheet.Cell(2, 6).Value = speciesFilter;

        worksheet.Cell(3, 5).Value = "Pagina";
        worksheet.Cell(3, 6).Value = page;

        worksheet.Cell(4, 5).Value = "Pokemon por pagina";
        worksheet.Cell(4, 6).Value = pageSize;

        // Define la fila donde comenzaran los Pokemon
        var row = 2;

        foreach (var currentPokemon in pokemon)
        {
            // Agrega el nombre del Pokemon
            worksheet.Cell(row, 1).Value =
                currentPokemon.Name;

            // Agrega la URL de la imagen
            worksheet.Cell(row, 2).Value =
                currentPokemon.ImageUrl;

            // Agrega la URL original de PokeAPI
            worksheet.Cell(row, 3).Value =
                currentPokemon.ApiUrl;

            // Avanza a la siguiente fila
            row++;
        }

        // Aplica formato a los encabezados
        worksheet.Range("A1:C1").Style.Font.Bold = true;

        // Ajusta el ancho de las columnas al contenido
        worksheet.Columns().AdjustToContents();

        // Guarda el libro en memoria
        using var stream = new MemoryStream();

        // Convierte el libro en bytes para descargarlo
        workbook.SaveAs(stream);

        // Devuelve el archivo Excel generado
        return stream.ToArray();
    }
}