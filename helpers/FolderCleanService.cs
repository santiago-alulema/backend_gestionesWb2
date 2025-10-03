namespace gestiones_backend.helpers
{
    public class FolderCleanService
    {
        private readonly string _basePath;

        public FolderCleanService(IWebHostEnvironment env)
        {
            _basePath = Path.Combine(env.ContentRootPath, "ArchivosExternos");
        }

        public void LimpiarCarpeta()
        {
            if (!Directory.Exists(_basePath))
                throw new DirectoryNotFoundException($"No se encontró la carpeta {_basePath}");

            foreach (var archivo in Directory.GetFiles(_basePath))
            {
                File.Delete(archivo);
            }

            foreach (var carpeta in Directory.GetDirectories(_basePath))
            {
                Directory.Delete(carpeta, recursive: true);
            }
        }
    }
}
