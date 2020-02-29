using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace TSensor.FillValues
{
    public class ExcelService
    {
        public Dictionary<int, Dictionary<int, string>> ReadToEnd(Stream stream)
        {
            var result = new Dictionary<int, Dictionary<int, string>>();

            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets[1];

                for (var r = sheet.Dimension.Start.Row; r <= sheet.Dimension.End.Row; r++)
                {
                    var row = new Dictionary<int, string>();

                    for (var c = sheet.Dimension.Start.Column; c <= sheet.Dimension.End.Column; c++)
                    {
                        row.Add(c, sheet.Cells[r, c].Text);
                    }

                    result.Add(r, row);
                }
            }

            return result;
        }

        private const string FIRST_SHEET_NAME = "Sheet 1";
        public void SaveToStream(IEnumerable<IEnumerable<string>> data, Stream stream)
        {
            using (var package = new ExcelPackage())
            {
                package.Workbook.Worksheets.Add(FIRST_SHEET_NAME);
                var sheet = package.Workbook.Worksheets[1];
                sheet.Name = FIRST_SHEET_NAME;

                var rowIndex = 1;
                foreach (var row in data)
                {
                    var colIndex = 1;
                    foreach (var cell in row)
                    {
                        sheet.Cells[rowIndex, colIndex].Value = cell;
                        colIndex++;
                    }

                    rowIndex++;
                }

                package.SaveAs(stream);
            }
        }
    }
}