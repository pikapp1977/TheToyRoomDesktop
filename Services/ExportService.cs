using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using TheToyRoomDesktop.Models;

namespace TheToyRoomDesktop.Services;

public class ExportService
{
    public byte[] ExportCollectionToExcel(List<Collectible> items, string title = "Collection Report")
    {
        using var memoryStream = new MemoryStream();
        using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            var sheet = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = title
            };
            sheets.Append(sheet);

            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

            // Add header row
            var headerRow = new Row { RowIndex = 1 };
            headerRow.Append(
                CreateCell("Name", CellValues.String),
                CreateCell("Character", CellValues.String),
                CreateCell("Deco", CellValues.String),
                CreateCell("Manufacturer", CellValues.String),
                CreateCell("Reissue", CellValues.String),
                CreateCell("Stylized", CellValues.String),
                CreateCell("Original Price", CellValues.String),
                CreateCell("Current Value", CellValues.String),
                CreateCell("Gain/Loss", CellValues.String),
                CreateCell("Date Acquired", CellValues.String),
                CreateCell("Date Added", CellValues.String),
                CreateCell("Notes", CellValues.String)
            );
            sheetData.Append(headerRow);

            // Add data rows
            uint rowIndex = 2;
            decimal totalOriginal = 0;
            decimal totalCurrent = 0;

            foreach (var item in items)
            {
                var gainLoss = item.CurrentValue - item.OriginalPrice;
                totalOriginal += item.OriginalPrice;
                totalCurrent += item.CurrentValue;

                var dataRow = new Row { RowIndex = rowIndex };
                dataRow.Append(
                    CreateCell(item.Name, CellValues.String),
                    CreateCell(item.Character ?? "", CellValues.String),
                    CreateCell(item.DecoName ?? "", CellValues.String),
                    CreateCell(item.Manufacturer, CellValues.String),
                    CreateCell(item.Reissue ? "Yes" : "No", CellValues.String),
                    CreateCell(item.Stylized ? "Yes" : "No", CellValues.String),
                    CreateCell(item.OriginalPrice.ToString("F2"), CellValues.Number),
                    CreateCell(item.CurrentValue.ToString("F2"), CellValues.Number),
                    CreateCell(gainLoss.ToString("F2"), CellValues.Number),
                    CreateCell(item.DateAcquired?.ToString("MM/dd/yyyy") ?? "N/A", CellValues.String),
                    CreateCell(item.DateAdded.ToString("MM/dd/yyyy"), CellValues.String),
                    CreateCell(item.Notes ?? "", CellValues.String)
                );
                sheetData.Append(dataRow);
                rowIndex++;
            }

            // Add totals row
            var totalGainLoss = totalCurrent - totalOriginal;
            var totalsRow = new Row { RowIndex = rowIndex };
            totalsRow.Append(
                CreateCell("", CellValues.String),
                CreateCell("", CellValues.String),
                CreateCell("", CellValues.String),
                CreateCell("", CellValues.String),
                CreateCell("", CellValues.String),
                CreateCell("TOTALS:", CellValues.String),
                CreateCell(totalOriginal.ToString("F2"), CellValues.Number),
                CreateCell(totalCurrent.ToString("F2"), CellValues.Number),
                CreateCell(totalGainLoss.ToString("F2"), CellValues.Number),
                CreateCell("", CellValues.String),
                CreateCell("", CellValues.String),
                CreateCell("", CellValues.String)
            );
            sheetData.Append(totalsRow);

            workbookPart.Workbook.Save();
        }

        return memoryStream.ToArray();
    }

    public byte[] GenerateImportTemplate()
    {
        using var memoryStream = new MemoryStream();
        using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
        {
            var workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            var sheet = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Import Template"
            };
            sheets.Append(sheet);

            var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

            // Add header row with instructions
            var headerRow = new Row { RowIndex = 1 };
            headerRow.Append(
                CreateCell("Name", CellValues.String),
                CreateCell("Character", CellValues.String),
                CreateCell("Deco", CellValues.String),
                CreateCell("Manufacturer", CellValues.String),
                CreateCell("Reissue", CellValues.String),
                CreateCell("Stylized", CellValues.String),
                CreateCell("Original Price", CellValues.String),
                CreateCell("Current Value", CellValues.String),
                CreateCell("Date Acquired", CellValues.String),
                CreateCell("Notes", CellValues.String)
            );
            sheetData.Append(headerRow);

            // Add example row
            var exampleRow = new Row { RowIndex = 2 };
            exampleRow.Append(
                CreateCell("Example Item", CellValues.String),
                CreateCell("Optimus Prime", CellValues.String),
                CreateCell("G1", CellValues.String),
                CreateCell("Hasbro", CellValues.String),
                CreateCell("No", CellValues.String),
                CreateCell("No", CellValues.String),
                CreateCell("29.99", CellValues.String),
                CreateCell("150.00", CellValues.String),
                CreateCell("01/15/2020", CellValues.String),
                CreateCell("Mint in box", CellValues.String)
            );
            sheetData.Append(exampleRow);

            workbookPart.Workbook.Save();
        }

        return memoryStream.ToArray();
    }

    public class ImportResult
    {
        public List<Collectible> Items { get; set; } = new();
        public List<string> SkippedRows { get; set; } = new();
    }

    public ImportResult ParseImportFile(Stream fileStream)
    {
        var result = new ImportResult();

        try
        {
            using (var document = SpreadsheetDocument.Open(fileStream, false))
            {
                var workbookPart = document.WorkbookPart;
                var worksheetPart = workbookPart?.WorksheetParts.First();
                var sheetData = worksheetPart?.Worksheet.GetFirstChild<SheetData>();

                if (sheetData == null)
                {
                    Console.WriteLine("ParseImportFile: sheetData is null");
                    return result;
                }

                var rows = sheetData.Elements<Row>().ToList();
                Console.WriteLine($"ParseImportFile: Found {rows.Count} rows");
                
                // Skip header row (row 1)
                for (int i = 1; i < rows.Count; i++)
                {
                    var row = rows[i];
                    var rowNumber = i + 1; // Excel row numbers start at 1, and we add 1 for header
                    var cells = row.Elements<Cell>().ToList();
                    Console.WriteLine($"ParseImportFile: Row {rowNumber} has {cells.Count} cells");

                    // Skip empty rows
                    if (cells.Count == 0) continue;

                    var name = GetCellValue(workbookPart, cells, 0);
                    var manufacturer = GetCellValue(workbookPart, cells, 3);
                    
                    Console.WriteLine($"ParseImportFile: Row {rowNumber} - Name: '{name}', Manufacturer: '{manufacturer}'");
                    
                    // Validate required fields
                    var missingFields = new List<string>();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        missingFields.Add("Name");
                    }
                    if (string.IsNullOrWhiteSpace(manufacturer))
                    {
                        missingFields.Add("Manufacturer");
                    }

                    if (missingFields.Any())
                    {
                        var reason = $"Row {rowNumber}: Missing {string.Join(", ", missingFields)}";
                        result.SkippedRows.Add(reason);
                        Console.WriteLine($"ParseImportFile: {reason}");
                        continue;
                    }

                    var item = new Collectible
                    {
                        Name = name!,
                        Character = GetCellValue(workbookPart, cells, 1),
                        Manufacturer = manufacturer!,
                        Reissue = GetCellValue(workbookPart, cells, 4)?.ToLower() == "yes",
                        Stylized = GetCellValue(workbookPart, cells, 5)?.ToLower() == "yes",
                        OriginalPrice = ParseDecimal(GetCellValue(workbookPart, cells, 6)),
                        CurrentValue = ParseDecimal(GetCellValue(workbookPart, cells, 7)),
                        DateAcquired = ParseDate(GetCellValue(workbookPart, cells, 8)),
                        Notes = GetCellValue(workbookPart, cells, 9),
                        DateAdded = DateTime.Now
                    };

                    // Store deco name temporarily (will be converted to ID later)
                    var decoName = GetCellValue(workbookPart, cells, 2);
                    if (!string.IsNullOrWhiteSpace(decoName))
                    {
                        item.DecoName = decoName;
                    }

                    Console.WriteLine($"ParseImportFile: Row {rowNumber} - Item added: {item.Name}");
                    result.Items.Add(item);
                }
                
                Console.WriteLine($"ParseImportFile: Total items parsed: {result.Items.Count}");
                Console.WriteLine($"ParseImportFile: Total rows skipped: {result.SkippedRows.Count}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ParseImportFile Error: {ex.Message}");
            Console.WriteLine($"ParseImportFile Stack: {ex.StackTrace}");
        }

        return result;
    }

    private string? GetCellValue(WorkbookPart? workbookPart, List<Cell> cells, int columnIndex)
    {
        // Excel cells might not be in sequential order, so we need to find by column reference
        // Column A=0, B=1, C=2, etc.
        var columnName = GetColumnName(columnIndex);
        
        var cell = cells.FirstOrDefault(c => 
            c.CellReference != null && 
            c.CellReference.Value != null && 
            c.CellReference.Value.ToString().StartsWith(columnName));
        
        if (cell == null || cell.CellValue == null)
        {
            Console.WriteLine($"GetCellValue: Column {columnIndex} ({columnName}) - no value");
            return null;
        }

        var value = cell.CellValue.Text;

        // Handle shared strings
        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            var stringTable = workbookPart?.SharedStringTablePart?.SharedStringTable;
            if (stringTable != null)
            {
                value = stringTable.ElementAt(int.Parse(value)).InnerText;
            }
        }

        Console.WriteLine($"GetCellValue: Column {columnIndex} ({columnName}) = '{value}'");
        return value;
    }

    private string GetColumnName(int columnIndex)
    {
        int dividend = columnIndex + 1;
        string columnName = String.Empty;

        while (dividend > 0)
        {
            int modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo) + columnName;
            dividend = (dividend - modulo) / 26;
        }

        return columnName;
    }

    private decimal ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        if (decimal.TryParse(value, out var result)) return result;
        return 0;
    }

    private DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "N/A") return null;
        if (DateTime.TryParse(value, out var result)) return result;
        return null;
    }

    private Cell CreateCell(string text, CellValues dataType)
    {
        return new Cell()
        {
            CellValue = new CellValue(text),
            DataType = new EnumValue<CellValues>(dataType)
        };
    }
}
