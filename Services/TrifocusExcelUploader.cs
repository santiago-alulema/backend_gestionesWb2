using ClosedXML.Excel;
using gestiones_backend.Class;
using gestiones_backend.DbConn;
using gestiones_backend.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using System.Data;

namespace gestiones_backend.Services
{
    public class TrifocusExcelUploader : ITrifocusExcelUploader
    {
        private readonly IWebHostEnvironment _env;
        private readonly SftpOptions _sftp;
        private readonly TrifocusExportOptions _exp;
        private readonly IConfiguration _configuration;

        private const string SqlTrifocus = @"
                                                SELECT 
                                                    d.""CodigoOperacion""  as ""NOPERACION"",
                                                    TO_CHAR(g.""FechaGestion""  , 'YYYY/MM/DD') as ""DFECHA_PROCESO"",
                                                    'LATICOB' as ""CCODIGO_GESTOR"",
                                                    tcr.""CodigoEmpresaExterna""  as ""CCODIGO_GESTION"",
                                                    TO_CHAR(d.""FechaRegistro"", 'YYYY/MM/DD') as ""DFECHA_ASIGNACION"",
                                                    TO_CHAR(d.""FechaRegistro"", 'hh:mm:ss') as ""CHORA_ASIGNACION"",
                                                    '' as ""CUSUARIO_ASIGNACION"",
                                                    '' as ""CTERMINAL_ASIGNACION"",
                                                    '001' as ""CCODIGO_TIPO_ASIGNACION"",
                                                    TO_CHAR(g.""FechaGestion""  , 'YYYY/MM/DD') as ""DFECHA_EJECUCION"",
                                                    TO_CHAR(g.""FechaGestion""  , 'hh:mm:ss') as ""CHORA_EJECUCION"",
                                                    '' as ""DFECHA_COMPROMISO_PAGO"",
                                                    '' as ""CHORA_COMPROMISO_PAGO"",
                                                    TO_CHAR(g.""FechaGestion""  , 'YYYY/MM/DD') as ""DFECHA_REGISTRA_RESULTADO"",
                                                    TO_CHAR(g.""FechaGestion""  , 'hh:mm:ss') as ""CHORA_REGISTRA_RESULTADO"",
                                                    '' as ""CUSUARIO_REGISTRA_RESULTADO"",
                                                    '' as ""CTERMINAL_REGISTRA_RESULTADO"",
                                                    rtc.""CodigoEmpresaExterna""  as ""CCODIGO_RESULTADO"",
                                                    '1' as ""LGESTION_TERMINADA"",
                                                    g.""Descripcion""  as ""COBSERVACION"",
                                                    '0' as ""FMONTO_ASIGNADO"",
                                                    d2.""CodigoDeudor""  as ""NCODIGO_CLIENTE""
                                                FROM ""Deudas"" d 
                                                join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor"" 
                                                join ""Gestiones"" g on g.""IdDeuda""  = d.""IdDeuda"" 
                                                left join ""RespuestasTipoContacto"" rtc on rtc.""Id"" = g.""IdRespuestaTipoContacto""  
                                                join ""TiposContactoResultado"" tcr on rtc.""IdTipoContactoResultado"" = tcr.""Id"" 
                                                where d.""Empresa"" = 'CRECOSCORP'

                                                union all

                                                SELECT 
                                                    d.""CodigoOperacion""  as ""NOPERACION"",
                                                    TO_CHAR(p.""FechaPago""  , 'YYYY/MM/DD') as ""DFECHA_PROCESO"",
                                                    'LATICOB' as ""CCODIGO_GESTOR"",
                                                    '010011'  as ""CCODIGO_GESTION"",
                                                    TO_CHAR(d.""FechaRegistro"", 'YYYY/MM/DD') as ""DFECHA_ASIGNACION"",
                                                    TO_CHAR(d.""FechaRegistro"", 'hh:mm:ss') as ""CHORA_ASIGNACION"",
                                                    '' as ""CUSUARIO_ASIGNACION"",
                                                    '' as ""CTERMINAL_ASIGNACION"",
                                                    '001' as ""CCODIGO_TIPO_ASIGNACION"",
                                                    TO_CHAR(p.""FechaPago""  , 'YYYY/MM/DD') as ""DFECHA_EJECUCION"",
                                                    TO_CHAR(p.""FechaPago""   , 'hh:mm:ss') as ""CHORA_EJECUCION"",
                                                    '' as ""DFECHA_COMPROMISO_PAGO"",
                                                    '' as ""CHORA_COMPROMISO_PAGO"",
                                                    TO_CHAR(p.""FechaPago""   , 'YYYY/MM/DD') as ""DFECHA_REGISTRA_RESULTADO"",
                                                    TO_CHAR(p.""FechaPago""   , 'hh:mm:ss') as ""CHORA_REGISTRA_RESULTADO"",
                                                    '' as ""CUSUARIO_REGISTRA_RESULTADO"",
                                                    '' as ""CTERMINAL_REGISTRA_RESULTADO"",
                                                    al.""CodigoExterno""  as ""CCODIGO_RESULTADO"",
                                                    '1' as ""LGESTION_TERMINADA"",
                                                    p.""Observaciones""  as ""COBSERVACION"",
                                                    '0' as ""FMONTO_ASIGNADO"",
                                                    d2.""CodigoDeudor""  as ""NCODIGO_CLIENTE""
                                                FROM ""Deudas"" d 
                                                join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor"" 
                                                join ""Pagos"" p ON p.""IdDeuda""  = d.""IdDeuda"" 
                                                left join ""AbonosLiquidacion"" al on al.""Id""  = p.""IdAbonoLiquidacion""
                                                where d.""Empresa"" = 'CRECOSCORP'

                                                union all

                                                SELECT 
                                                    d.""CodigoOperacion""  as ""NOPERACION"",
                                                    TO_CHAR(cp.""FechaRegistro""  , 'YYYY/MM/DD') as ""DFECHA_PROCESO"",
                                                    'LATICOB' as ""CCODIGO_GESTOR"",
                                                    '010011'  as ""CCODIGO_GESTION"",
                                                    TO_CHAR(cp.""FechaRegistro"", 'YYYY/MM/DD') as ""DFECHA_ASIGNACION"",
                                                    TO_CHAR(cp.""FechaRegistro"", 'hh:mm:ss') as ""CHORA_ASIGNACION"",
                                                    '' as ""CUSUARIO_ASIGNACION"",
                                                    '' as ""CTERMINAL_ASIGNACION"",
                                                    '001' as ""CCODIGO_TIPO_ASIGNACION"",
                                                    TO_CHAR(cp.""FechaCompromiso""  , 'YYYY/MM/DD') as ""DFECHA_EJECUCION"",
                                                    TO_CHAR(cp.""FechaCompromiso""   , 'hh:mm:ss') as ""CHORA_EJECUCION"",
                                                    '' as ""DFECHA_COMPROMISO_PAGO"",
                                                    '' as ""CHORA_COMPROMISO_PAGO"",
                                                    TO_CHAR(cp.""FechaRegistro""  , 'YYYY/MM/DD') as ""DFECHA_REGISTRA_RESULTADO"",
                                                    TO_CHAR(cp.""FechaRegistro""  , 'hh:mm:ss') as ""CHORA_REGISTRA_RESULTADO"",
                                                    '' as ""CUSUARIO_REGISTRA_RESULTADO"",
                                                    '' as ""CTERMINAL_REGISTRA_RESULTADO"",
                                                    tt.""CodigoExterno""  as ""CCODIGO_RESULTADO"",
                                                    '1' as ""LGESTION_TERMINADA"",
                                                    cp.""Observaciones""  as ""COBSERVACION"",
                                                    '0' as ""FMONTO_ASIGNADO"",
                                                    d2.""CodigoDeudor""  as ""NCODIGO_CLIENTE""
                                                FROM ""Deudas"" d 
                                                join ""Deudores"" d2 on d2.""IdDeudor"" = d.""IdDeudor"" 
                                                join ""CompromisosPagos"" cp ON cp.""IdDeuda""  = d.""IdDeuda"" 
                                                left join ""TiposTareas"" tt on tt.""Id""  = cp.""IdTipoTarea"" 
                                                where d.""Empresa"" = 'CRECOSCORP';
                                                ";

        public TrifocusExcelUploader(
            IWebHostEnvironment env,
            IOptions<SftpOptions> sftp,
            IOptions<TrifocusExportOptions> exp,
            IConfiguration configuration)
        {
            _env = env;
            _sftp = sftp.Value;
            _exp = exp.Value;
            _configuration= configuration;
            // Aseguramos que PgConn tenga su cadena

        }

        public async Task<string> GenerateAndUploadAsync(CancellationToken ct = default)
        {
            // 1) Fecha y rutas
            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil");
            var nowEc = TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz);
            var yyyymmdd = nowEc.ToString("yyyyMMdd");

            var baseDir = Path.Combine(_env.ContentRootPath, "ArchivosExternos", _exp.SubFolderName);
            Directory.CreateDirectory(baseDir);

            var localPath = Path.Combine(baseDir, $"GESTIONES_TRIFOCUS_{yyyymmdd}.xlsx");
            if (File.Exists(localPath)) File.Delete(localPath);

            // 2) Consulta con TU PgConn
            PgConn _pg = new PgConn();
            _pg.cadenaConnect = _configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection no configurada.");
            DataTable dt = _pg.ejecutarconsulta_dt(SqlTrifocus, timeout: 600);

            // 3) Excel
            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet("GESTIONES");
                ws.Cell(1, 1).InsertTable(dt, "Datos", true);

                var used = ws.RangeUsed();
                if (used != null)
                {
                    used.Style.NumberFormat.Format = "@"; // todo como texto
                    ws.SheetView.FreezeRows(1);
                    ws.Columns().AdjustToContents();
                }

                wb.SaveAs(localPath);
            }

            ct.ThrowIfCancellationRequested();
            string pathServicio = _sftp.RemotePath + "/Archivos Diarios Trifocus";

            // 4) SFTP
            using (var sftp = new SftpClient(_sftp.Host, _sftp.Port, _sftp.Username, _sftp.Password))
            {
                sftp.Connect();
                EnsureRemoteDir(sftp, pathServicio);

                var remoteFile = $"{pathServicio.TrimEnd('/')}/{Path.GetFileName(localPath)}";
                using var fs = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                sftp.UploadFile(fs, remoteFile, true); // sobrescribe
                sftp.Disconnect();
            }

            return await Task.FromResult(localPath);
        }

        private static void EnsureRemoteDir(SftpClient sftp, string remotePath)
        {
            if (string.IsNullOrWhiteSpace(remotePath)) return;

            string[] parts = remotePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string current = "/";
            foreach (var part in parts)
            {
                current += part;
                if (!sftp.Exists(current))
                    sftp.CreateDirectory(current);
                current += "/";
            }
        }
    }
}