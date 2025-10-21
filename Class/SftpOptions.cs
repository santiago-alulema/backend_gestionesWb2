namespace gestiones_backend.Class
{
    public class SftpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 22;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string RemotePath { get; set; } = "/";
    }
}
