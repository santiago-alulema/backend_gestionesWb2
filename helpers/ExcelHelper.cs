using OfficeOpenXml;
using System.Reflection;

namespace gestiones_backend.helpers
{
    public class ExcelHelper
    {
        public static byte[] ListToExcel<T>(List<T> data, string sheetName = "Sheet1")
        {
            ExcelPackage.License.SetNonCommercialPersonal("Santiago");

            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add(sheetName);

                // Obtiene las propiedades públicas de la clase T
                var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                // Escribe los encabezados
                for (int i = 0; i < props.Length; i++)
                {
                    ws.Cells[1, i + 1].Value = props[i].Name;
                }

                // Escribe los datos
                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < props.Length; col++)
                    {
                        var value = props[col].GetValue(data[row]);
                        ws.Cells[row + 2, col + 1].Value = value;
                    }
                }

                // Autoajuste de columnas
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}
