using System.IO.Compression;

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
    }
}
