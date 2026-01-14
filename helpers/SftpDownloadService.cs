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

        public DateTime? ObtenerFechaUltimaCarteraAsignadaUtc()
        {
            using var client = new SftpClient(_host, _port, _username, _password);

            try
            {
                client.Connect();

                var ultimoArchivo = client.ListDirectory(_remotePath)
                    .Where(f =>
                        !f.IsDirectory &&
                        f.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) &&
                        f.Name.StartsWith("Archivo Asignacion Cartera", StringComparison.OrdinalIgnoreCase)
                    )
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (ultimoArchivo == null) return null;

                var dt = ultimoArchivo.LastWriteTime;

                // convertir a UTC para timestamptz
                return dt.Kind switch
                {
                    DateTimeKind.Utc => dt,
                    DateTimeKind.Local => dt.ToUniversalTime(),
                    _ => DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime()
                };
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

                var files = client.ListDirectory(_remotePath)
                    .Where(f => !f.IsDirectory && f.Name.EndsWith(".zip"))
                    .ToList();

                if (!files.Any())
                    return;

                Directory.CreateDirectory(_localPath);

                var ultimoAsignacion = files
                    .Where(f => f.Name.StartsWith("Archivo Asignacion Cartera", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (ultimoAsignacion != null)
                {
                    using var fs = new FileStream(Path.Combine(_localPath, ultimoAsignacion.Name), FileMode.Create);
                    client.DownloadFile(ultimoAsignacion.FullName, fs);
                }

                var ultimoDiario = files
                    .Where(f => f.Name.StartsWith("Archivos Diarios", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (ultimoDiario != null)
                {
                    using var fs = new FileStream(Path.Combine(_localPath, ultimoDiario.Name), FileMode.Create);
                    client.DownloadFile(ultimoDiario.FullName, fs);
                }
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
