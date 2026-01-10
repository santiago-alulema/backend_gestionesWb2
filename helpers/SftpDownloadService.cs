using gestiones_backend.Class;
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
            _localPath = Path.Combine(env.ContentRootPath, "ArchivosExternos");

            if (!Directory.Exists(_localPath))
                Directory.CreateDirectory(_localPath);
        }

        public List<NombreArchivosCrecos> ObtenerNombresArchivos()
        {
            using var client = new SftpClient(_host, _port, _username, _password);
            List<NombreArchivosCrecos> listaNombresCrecos = new List<NombreArchivosCrecos>();
            try
            {
                client.Connect();
                Console.WriteLine("Conexión SFTP establecida.");

                var files = client.ListDirectory(_remotePath);

                foreach (var file in files)
                {
                    if (!file.IsDirectory && file.Name.EndsWith(".zip"))
                    {
                        listaNombresCrecos.Add(new NombreArchivosCrecos() { id = file.Name, name = file.Name });
                    }
                }
                return listaNombresCrecos;
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

        public (byte[] FileBytes, string FileName)? DescargarArchivoEspecifico(string nombreEspecifico)
        {
            using var client = new SftpClient(_host, _port, _username, _password);

            try
            {
                client.Connect();
                var files = client.ListDirectory(_remotePath);

                foreach (var file in files)
                {
                    if (!file.IsDirectory && file.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        if (file.Name.Contains(nombreEspecifico, StringComparison.OrdinalIgnoreCase))
                        {
                            using var ms = new MemoryStream();
                            client.DownloadFile(file.FullName, ms);

                            return (ms.ToArray(), file.Name);
                        }
                    }
                }

                return null;
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

        public void DescargarZipsUltimoDia()
        {
            using var client = new SftpClient(_host, _port, _username, _password);

            try
            {
                client.Connect();
                Console.WriteLine("Conexión SFTP establecida.");

                var zipFiles = client
                    .ListDirectory(_remotePath)
                    .Where(f => !f.IsDirectory && f.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!zipFiles.Any())
                {
                    Console.WriteLine("No se encontraron archivos .zip para descargar.");
                    return;
                }

                // 1) Detectar el día más reciente (por día)
                var latestDate = zipFiles.Max(f => f.LastWriteTime.Date);

                // 2) Filtrar solo los archivos de ese día
                var latestZips = zipFiles
                    .Where(f => f.LastWriteTime.Date == latestDate)
                    .OrderByDescending(f => f.LastWriteTime)
                    .ToList();

                Directory.CreateDirectory(_localPath);

                foreach (var file in latestZips)
                {
                    string localFile = Path.Combine(_localPath, file.Name);

                    using var fs = new FileStream(localFile, FileMode.Create, FileAccess.Write);
                    client.DownloadFile(file.FullName, fs);

                    Console.WriteLine($"Archivo descargado: {file.Name} (Fecha: {file.LastWriteTime:yyyy-MM-dd HH:mm:ss})");
                }

                Console.WriteLine($"Descarga completada. Día más reciente: {latestDate:yyyy-MM-dd}. Total: {latestZips.Count}");
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


        public void DescargarZips()
        {
            using var client = new SftpClient(_host, _port, _username, _password);

            try
            {
                client.Connect();
                Console.WriteLine("Conexión SFTP establecida.");

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
