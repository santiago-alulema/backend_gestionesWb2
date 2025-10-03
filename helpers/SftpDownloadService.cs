using Renci.SshNet;

namespace gestiones_backend.helpers
{
    public class SftpDownloadService
    {
        private readonly string _host = "186.3.14.45";
        //private readonly string _host = "localhost";
       
        private readonly int _port = 34;
        //private readonly int _port = 9093;


        private readonly string _username = "ftp.trifocus";
        private readonly string _password = "Cr3sa.25tri";
        private readonly string _remotePath = "/Trifocus";
        private readonly string _localPath;

        public SftpDownloadService(IWebHostEnvironment env)
        {
            // Carpeta ArchivosExternos dentro de tu proyecto
            _localPath = Path.Combine(env.ContentRootPath, "ArchivosExternos");

            if (!Directory.Exists(_localPath))
                Directory.CreateDirectory(_localPath);
        }

        public void DescargarZips()
        {
            using var client = new SftpClient(_host, _port, _username, _password);

            try
            {
                client.Connect();
                Console.WriteLine("Conexión SFTP establecida.");

                // Obtener todos los archivos en la ruta remota
                var files = client.ListDirectory(_remotePath);

                foreach (var file in files)
                {
                    if (!file.IsDirectory && file.Name.EndsWith(".zip"))
                    {
                        string localFile = Path.Combine(_localPath, file.Name);

                        using var fs = new FileStream(localFile, FileMode.Create);
                        client.DownloadFile(file.FullName, fs);

                        Console.WriteLine($"Archivo descargado: {file.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la descarga SFTP: {ex.Message}");
                throw;
            }
            finally
            {
                if (client.IsConnected)
                    client.Disconnect();
            }
        }
    }
}
