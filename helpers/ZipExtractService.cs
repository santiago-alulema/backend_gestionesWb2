using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace gestiones_backend.helpers
{
    public class ZipExtractService
    {
        private readonly string _basePath;

        public ZipExtractService(IWebHostEnvironment env)
        {
            _basePath = Path.Combine(env.ContentRootPath, "ArchivosExternos");
        }

        public void DescomprimirZips()
        {
            if (!Directory.Exists(_basePath))
                throw new DirectoryNotFoundException($"No se encontró la carpeta {_basePath}");

            var archivosZip = Directory.GetFiles(_basePath, "*.zip");

            foreach (var zip in archivosZip)
            {
                string nombreArchivo = Path.GetFileNameWithoutExtension(zip);
                string carpetaDestino = Path.Combine(_basePath, nombreArchivo);

                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                // Descomprimir
                ZipFile.ExtractToDirectory(zip, carpetaDestino, overwriteFiles: true);

                Console.WriteLine($"Archivo {zip} descomprimido en {carpetaDestino}");
            }
        }

        public void DescomprimirZipsUltimo()
        {
            if (!Directory.Exists(_basePath))
                throw new DirectoryNotFoundException($"No se encontró la carpeta {_basePath}");

            var archivosZip = Directory.GetFiles(_basePath, "*.zip");
            if (archivosZip.Length == 0)
            {
                Console.WriteLine("No se encontraron archivos ZIP en la carpeta.");
                return;
            }

            // Regex: <base> <yyyy-MM-dd> <H>-<mm>
            // Ej: "Archivos Diarios 2025-10-07 0-16"
            var rx = new Regex(@"^(?<base>.+?)\s(?<fecha>\d{4}-\d{2}-\d{2})\s(?<hora>\d{1,2})-(?<min>\d{2})$",
                RegexOptions.Compiled);

            var items = archivosZip
                .Select(ruta =>
                {
                    string nombreSinExt = Path.GetFileNameWithoutExtension(ruta);
                    var m = rx.Match(nombreSinExt);
                    if (m.Success)
                    {
                        string baseName = m.Groups["base"].Value.Trim();
                        string fecha = m.Groups["fecha"].Value;          // yyyy-MM-dd
                        string hora = m.Groups["hora"].Value;            // H (1-2 dígitos)
                        string min = m.Groups["min"].Value;             // mm

                        // Construimos "yyyy-MM-dd H:mm" con H de 24h
                        var ok = DateTime.TryParseExact(
                            $"{fecha} {hora}:{min}",
                            "yyyy-MM-dd H:mm",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var dt);

                        return new
                        {
                            Ruta = ruta,
                            MatchOk = ok,
                            Base = baseName,
                            Fecha = ok ? dt : File.GetLastWriteTime(ruta) // fallback
                        };
                    }
                    else
                    {
                        // Si no matchea (caso raro), agrupamos por nombre completo sin fecha
                        return new
                        {
                            Ruta = ruta,
                            MatchOk = false,
                            Base = nombreSinExt, // agrupa por el nombre completo
                            Fecha = File.GetLastWriteTime(ruta)
                        };
                    }
                })
                .ToList();

            // Agrupar por base y tomar el más reciente por Fecha
            var grupos = items.GroupBy(x => x.Base);

            foreach (var g in grupos)
            {
                var masReciente = g.OrderByDescending(x => x.Fecha).First();

                string carpetaDestino = Path.Combine(_basePath, g.Key); // carpeta = nombre base (sin fecha)
                if (!Directory.Exists(carpetaDestino))
                    Directory.CreateDirectory(carpetaDestino);

                ZipFile.ExtractToDirectory(masReciente.Ruta, carpetaDestino, overwriteFiles: true);

                Console.WriteLine($"✅ Descomprimido: {Path.GetFileName(masReciente.Ruta)} → {carpetaDestino}");
            }
        }

        private static DateTime ExtraerFechaDesdeNombre(string ruta)
        {
            string nombre = Path.GetFileNameWithoutExtension(ruta);
            string[] partes = nombre.Split('_');

            if (partes.Length > 1)
            {
                string posibleFecha = partes[^1]; // toma la parte después del último "_"
                if (DateTime.TryParseExact(posibleFecha, "yyyyMMdd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var fecha))
                {
                    return fecha;
                }
            }

            // Si no se puede parsear, usa la fecha de modificación del archivo
            return File.GetLastWriteTime(ruta);
        }
    }
}
