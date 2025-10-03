using gestiones_backend.Context;
using gestiones_backend.Entity;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Services
{
    public class DeudoresImportService
    {
        private readonly DataContext _db;
        private readonly string _root;

        public DeudoresImportService(DataContext db, IWebHostEnvironment env)
        {
            _db = db;
            _root = Path.Combine(env.ContentRootPath, "ArchivosExternos");
        }

        public async Task<int> ImportarDeudoresCompletoAsync()
        {
            if (!Directory.Exists(_root))
                throw new DirectoryNotFoundException($"No existe la carpeta: {_root}");

            var datosCliente = new ConcurrentDictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var direccionCliente = new ConcurrentDictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            var files = Directory.EnumerateFiles(_root, "*.*", SearchOption.AllDirectories)
                                 .Where(f =>
                                     f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                     f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var upperName = Path.GetFileNameWithoutExtension(file).ToUpperInvariant();

                if (upperName.Contains("DATOSCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        if (!row.TryGetValue("CNUMEROIDENTIFICACION", out var codigo) || string.IsNullOrWhiteSpace(codigo))
                            continue;
                        datosCliente[codigo] = row; // último gana
                    }
                }
                else if (upperName.Contains("DIRECCIONCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        if (!row.TryGetValue("CNUMEROIDENTIFICACION", out var codigo) || string.IsNullOrWhiteSpace(codigo))
                            continue;
                        direccionCliente[codigo] = row; // último gana
                    }
                }
            }

            var candidatos = new List<Deudores>();

            foreach (var kv in datosCliente)
            {
                var codigo = kv.Key;
                var d = kv.Value;  
                direccionCliente.TryGetValue(codigo, out var dir); // puede no existir

                d.TryGetValue("CNUMEROIDENTIFICACION", out var idDeudor);

                d.TryGetValue("CNOMBRECOMPLETO", out var nombreCompleto);
                d.TryGetValue("NOMBRES", out var nombres);
                d.TryGetValue("APELLIDOS", out var apellidos);

                d.TryGetValue("CNOMBREEMPRESA", out var empresa);

                d.TryGetValue("REFERENCIA", out var referencia);
                d.TryGetValue("DESCRIP_TITULO", out var descripTitulo);

                var direccion =  Get(dir, "CDIRECCIONCOMPLETA") ;
                var telefono = Get(dir, "TELEFONO");
                var email = Get(dir, "CCORREOELECTRONICO");

                var codigoDeudor = Get(d, "ICODIGOCLIENTE");

                if (string.IsNullOrWhiteSpace(idDeudor))
                    continue;

                var nombre = !string.IsNullOrWhiteSpace(nombreCompleto)
                    ? nombreCompleto
                    : string.Join(" ", new[] { nombres, apellidos }.Where(s => !string.IsNullOrWhiteSpace(s)));

                var descripcion = Get(dir, "COBSERVACION");

                var entidad = new Deudores
                {
                    IdDeudor = idDeudor!.Trim(),
                    Nombre = (nombre ?? "").Trim(),
                    Direccion = direccion?.Trim(),
                    Empresa = "CRECOSCORP",
                    Telefono = telefono?.Trim(),
                    Correo = email?.Trim(),
                    Descripcion = descripcion?.Trim(),
                    CodigoDeudor = codigoDeudor?.Trim(),
                    FechaRegistro = DateTime.Now
                };

                candidatos.Add(entidad);
            }

            if (candidatos.Count == 0) return 0;

            var ids = candidatos.Select(c => c.IdDeudor).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var existentes = await _db.Set<Deudores>()
                                      .Where(d => ids.Contains(d.IdDeudor))
                                      .ToDictionaryAsync(d => d.IdDeudor, StringComparer.OrdinalIgnoreCase);

            var nuevos = new List<Deudores>();

            foreach (var c in candidatos)
            {
                if (existentes.TryGetValue(c.IdDeudor, out var e))
                {
                    e.Nombre = c.Nombre;
                    e.Direccion = c.Direccion;
                    e.Empresa = c.Empresa;
                    e.Telefono = c.Telefono;
                    e.Correo = c.Correo;
                    e.Descripcion = c.Descripcion;
                    e.CodigoDeudor = string.IsNullOrWhiteSpace(e.CodigoDeudor) ? c.CodigoDeudor : e.CodigoDeudor;
                    e.FechaRegistro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    e.IdUsuario = null;
                }
                else
                {
                    c.FechaRegistro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    c.IdUsuario = null;
                    nuevos.Add(c);
                }
            }

            if (nuevos.Count > 0)
                await _db.Set<Deudores>().AddRangeAsync(nuevos);

            return await _db.SaveChangesAsync();
        }


        public async Task<int> ImportarTelefonosBasicoAsync()
        {
            if (!Directory.Exists(_root))
                throw new DirectoryNotFoundException($"No existe la carpeta: {_root}");

            var files = Directory.EnumerateFiles(_root, "*.*", SearchOption.AllDirectories)
                                 .Where(f =>
                                     (f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                      f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)) &&
                                     Path.GetFileNameWithoutExtension(f).ToUpperInvariant().Contains("TELEFONOCLIENTE"));

            // 1) Recolectar candidatos (en memoria, sin tocar BD aún)
            var candidatos = new List<(string IdDeudor, string Telefono, string? Origen, string? Observacion, string? Propietario)>();
            foreach (var file in files)
            {
                foreach (var row in ReadDelimited(file))
                {
                    var idDeudor = Get(row, "CNUMEROIDENTIFICACION");
                    var numero = Get(row, "CNUMERO");
                    var prefijo = Get(row, "CPREFIJO");

                    if (string.IsNullOrWhiteSpace(idDeudor) || string.IsNullOrWhiteSpace(numero))
                        continue;

                    idDeudor = idDeudor.Trim();
                    var telefono = string.IsNullOrWhiteSpace(prefijo) ? numero : $"{prefijo}{numero}";
                    telefono = telefono.Trim();

                    // Datos opcionales
                    var origen = Get(row, "DESCRIP_UBICACION"); // Domicilio/Laboral/etc.
                    var observacion = "[MIGRADO SSH]";
                    var propietario = "deudor";

                    candidatos.Add((idDeudor, telefono, origen, observacion, propietario));
                }
            }

            if (candidatos.Count == 0) return 0;

            // 2) Cargar en memoria solo los deudores involucrados
            var idsDeArchivo = candidatos.Select(c => c.IdDeudor)
                                         .Distinct(StringComparer.OrdinalIgnoreCase)
                                         .ToList();

            var deudoresExistentes = await _db.Set<Deudores>()
                .Where(d => idsDeArchivo.Contains(d.IdDeudor))
                .Select(d => d.IdDeudor)
                .ToListAsync();

            var setDeudores = new HashSet<string>(deudoresExistentes, StringComparer.OrdinalIgnoreCase);

            // 3) Cargar teléfonos existentes para esos deudores (para evitar duplicados contra BD)
            var existentes = await _db.Set<DeudorTelefono>()
                .Where(t => t.IdDeudor != null &&
                            idsDeArchivo.Contains(t.IdDeudor))
                .Select(t => new { t.IdDeudor, t.Telefono })
                .ToListAsync();

            // Usamos una clave simple "id|tel" para comparación case-insensitive
            string MakeKey(string id, string tel) => $"{id}|{tel}";
            var setExistentes = new HashSet<string>(
                existentes.Select(e => MakeKey(e.IdDeudor!, e.Telefono!)),
                StringComparer.OrdinalIgnoreCase
            );

            // 4) Dedupe dentro del mismo lote y descartar los que no tienen deudor en BD
            var setLote = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var nuevos = new List<DeudorTelefono>();

            foreach (var c in candidatos)
            {
                if (!setDeudores.Contains(c.IdDeudor))
                    continue; // deudor no existe en BD

                var key = MakeKey(c.IdDeudor, c.Telefono);

                if (setExistentes.Contains(key))
                    continue; // ya existe en BD

                if (!setLote.Add(key))
                    continue; // repetido dentro del mismo lote

                nuevos.Add(new DeudorTelefono
                {
                    IdDeudorTelefonos = Guid.NewGuid().ToString("N"),
                    IdDeudor = c.IdDeudor,
                    Telefono = c.Telefono,
                    FechaAdicion = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    EsValido = true,
                    Propietario = c.Propietario,
                    Origen = c.Origen,
                    Observacion = c.Observacion
                });
            }

            if (nuevos.Count == 0) return 0;

            await _db.Set<DeudorTelefono>().AddRangeAsync(nuevos);
            return await _db.SaveChangesAsync();
        }




        public async Task<int> ImportarDeudasBasicoAsync()
        {
            if (!Directory.Exists(_root))
                throw new DirectoryNotFoundException($"No existe la carpeta: {_root}");

            // Índices simples en memoria
            var carteraById = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var operacionesByICodigo = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase); // ICODIGOOPERACION
            var articulosPorFactura = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);               // NUM_FACTURA
            var articulosPorCodOperacion = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);               // COD_OPERACION
            var saldos = new List<Dictionary<string, string>>();

            // 1) Leer archivos
            var files = Directory.EnumerateFiles(_root, "*.*", SearchOption.AllDirectories)
                                 .Where(f => f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                             f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file).ToUpperInvariant();

                if (name.Contains("CARTERAASIGNADA"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        var id = Get(row, "CNUMEROIDENTIFICACION");
                        if (!string.IsNullOrWhiteSpace(id))
                            carteraById[id.Trim()] = row;
                    }
                }
                else if (name.Contains("OPERACIONESCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        // Clave: ICODIGOOPERACION
                        var iCodOp = GetAny(row, "ICODIGOOPERACION", "I_CODIGO_OPERACION", "ICODIGO_OP");
                        if (!string.IsNullOrWhiteSpace(iCodOp))
                            operacionesByICodigo[iCodOp.Trim()] = row;
                    }
                }
                else if (name.Contains("ARTICULOOPERACION"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        var factura = GetAny(row, "NUM_FACTURA", "NRO_FACTURA", "NUMEROFACTURA");
                        var codOp = GetAny(row, "COD_OPERACION");
                        var prod = GetAny(row, "DESCRIP_ARTICULO", "DESCRIPCION_ARTICULO", "PRODUCTO", "ARTICULO", "NOMBRE");

                        if (!string.IsNullOrWhiteSpace(factura) && !string.IsNullOrWhiteSpace(prod))
                        {
                            var key = factura!.Trim();
                            if (!articulosPorFactura.TryGetValue(key, out var listF)) articulosPorFactura[key] = listF = new List<string>();
                            listF.Add(prod!.Trim());
                        }

                        if (!string.IsNullOrWhiteSpace(codOp) && !string.IsNullOrWhiteSpace(prod))
                        {
                            var key = codOp!.Trim();
                            if (!articulosPorCodOperacion.TryGetValue(key, out var listO)) articulosPorCodOperacion[key] = listO = new List<string>();
                            listO.Add(prod!.Trim());
                        }
                    }
                }
                else if (name.Contains("SALDOCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                        saldos.Add(row);
                }
            }

            if (saldos.Count == 0) return 0;

            // 2) Construir entidades Deuda (básico, sin deduplicar)
            var nuevos = new List<Deuda>();
            var ahoraUtc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            foreach (var s in saldos)
            {
                var idDeudor = Get(s, "CNUMEROIDENTIFICACION")?.Trim();
                if (string.IsNullOrWhiteSpace(idDeudor)) continue;

                var numeroFactura = GetAny(s, "NUM_FACTURA", "NRO_FACTURA", "NUMEROFACTURA")?.Trim();
                var codOperacionSal = GetAny(s, "COD_OPERACION", "CODOPERACION")?.Trim(); // en SALDOCLIENTE

                // Buscar fila de OPERACIONES usando ICODIGOOPERACION (= COD_OPERACION de ARTICULOOPERACION)
                operacionesByICodigo.TryGetValue(codOperacionSal ?? "", out var opRow);

                // Agencia/Ciudad desde cartera
                carteraById.TryGetValue(idDeudor, out var carRow);

                // ProductoDescripcion:
                // 1) por factura si existe
                // 2) si no hay factura, por COD_OPERACION (respetando ICODIGOOPERACION = COD_OPERACION)
                string? productoDescripcion = null;
                if (!string.IsNullOrWhiteSpace(numeroFactura) &&
                    articulosPorFactura.TryGetValue(numeroFactura!, out var prodsF) &&
                    prodsF.Count > 0)
                {
                    productoDescripcion = string.Join(" || ", prodsF);
                }
                else if (!string.IsNullOrWhiteSpace(codOperacionSal) &&
                         articulosPorCodOperacion.TryGetValue(codOperacionSal!, out var prodsO) &&
                         prodsO.Count > 0)
                {
                    productoDescripcion = string.Join(" || ", prodsO);
                }

                var deuda = new Deuda
                {
                    IdDeuda = Guid.NewGuid(),
                    IdDeudor = idDeudor,
                    DeudaCapital = ToDecimal(GetAny(s, "DEUDA_CAPITAL", "CAPITAL")),
                    Interes = ToDecimal(GetAny(s, "INTERES", "MORA")),
                    GastosCobranzas = ToDecimal(GetAny(s, "GASTOS_COBRANZAS", "GASTOS_COBRO")),
                    SaldoDeuda = ToDecimal(GetAny(s, "SALDO_DEUDA", "SALDO")),
                    Descuento = ToInt(GetAny(s, "DESCUENTO")),
                    MontoCobrar = ToDecimal(GetAny(s, "MONTO_COBRAR", "TOTAL_COBRAR")),
                    FechaVenta = ToDateOnly(GetAny(s, "FECHA_VENTA", "FECHAEMISION")) ?? ToDateOnly(GetAny(opRow, "FECHA_VENTA", "FECHA_EMISION")),
                    FechaUltimoPago = ToDateOnly(GetAny(s, "FECHA_ULTIMO_PAGO")),
                    Estado = GetAny(s, "ESTADO", "ESTADO_DEUDA"),
                    DiasMora = ToInt(GetAny(s, "DIAS_MORA")),
                    NumeroFactura = numeroFactura,
                    Clasificacion = GetAny(s, "CLASIFICACION"),
                    Creditos = ToInt(GetAny(s, "CREDITOS")),
                    SaldoDeulda = ToDecimal(GetAny(s, "SALDO_DEULDA")), // si existe esa columna
                    NumeroCuotas = ToInt(GetAny(s, "NUM_CUOTAS", "CUOTAS")),
                    TipoDocumento = GetAny(s, "TIPO_DOCUMENTO", "TIPODOC") ?? GetAny(opRow, "TIPO_DOCUMENTO", "TIPODOC"),
                    ValorCuota = ToDecimal(GetAny(s, "VALOR_CUOTA", "CUOTA")),
                    Tramo = GetAny(s, "TRAMO"),
                    UltimoPago = ToDecimal(GetAny(s, "ULTIMO_PAGO")),
                    ProductoDescripcion = productoDescripcion,
                    Agencia = GetAny(carRow, "AGENCIA", "NOM_AGENCIA"),
                    Ciudad = GetAny(carRow, "CIUDAD"),
                    Empresa = GetAny(s, "EMPRESA") ?? GetAny(opRow, "EMPRESA"),
                    CodigoEmpresa = GetAny(s, "COD_EMPRESA", "CODEMPRESA") ?? GetAny(opRow, "COD_EMPRESA", "CODEMPRESA"),
                    EsActivo = true,
                    FechaRegistro = ahoraUtc,
                    IdUsuario = null,
                    CodigoOperacion = codOperacionSal // guarda el mismo código usado para enlazar
                };

                // Calcular MontoCobrar si no vino
                if (deuda.MontoCobrar == null)
                {
                    var suma = (deuda.DeudaCapital ?? 0m) + (deuda.Interes ?? 0m) + (deuda.GastosCobranzas ?? 0m);
                    deuda.MontoCobrar = suma > 0 ? suma : deuda.SaldoDeuda;
                }

                nuevos.Add(deuda);
            }

            if (nuevos.Count == 0) return 0;

            await _db.Set<Deuda>().AddRangeAsync(nuevos);
            return await _db.SaveChangesAsync();

            // --------- Helpers locales mínimos ----------
            static string? GetAny(Dictionary<string, string>? row, params string[] cols)
            {
                if (row == null) return null;
                foreach (var c in cols)
                {
                    var v = Get(row, c);
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
                return null;
            }

            static decimal? ToDecimal(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                s = s.Trim().Replace(" ", "").Replace(",", ".");
                return decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : (decimal?)null;
            }

            static int? ToInt(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                return int.TryParse(s.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i) ? i : (int?)null;
            }

            static DateOnly? ToDateOnly(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (DateTime.TryParse(s.Trim(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt))
                    return DateOnly.FromDateTime(dt);
                return null;
            }
        }



        // =================== LECTOR TXT/CSV ROBUSTO ===================
        private static IEnumerable<Dictionary<string, string>> ReadDelimited(string path)
        {
            // Intentamos con UTF-8 y Latin1 (algunos TXT vienen ISO-8859-1)
            foreach (var enc in new[] { new UTF8Encoding(false), Encoding.GetEncoding("ISO-8859-1") })
            {
                try
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var sr = new StreamReader(fs, enc, detectEncodingFromByteOrderMarks: true);

                    string? headerLine = sr.ReadLine();
                    if (string.IsNullOrEmpty(headerLine)) yield break;

                    var delimiter = DetectDelimiter(headerLine);

                    // Reiniciar para que CsvHelper lea desde el inicio
                    fs.Seek(0, SeekOrigin.Begin);
                    using var sr2 = new StreamReader(fs, enc, detectEncodingFromByteOrderMarks: true);

                    var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = delimiter,
                        BadDataFound = null,
                        MissingFieldFound = null,
                        HeaderValidated = null,
                        IgnoreBlankLines = true,
                        TrimOptions = TrimOptions.Trim
                    };

                    using var csv = new CsvReader(sr2, cfg);
                    if (!csv.Read() || !csv.ReadHeader()) yield break;

                    var headers = csv.HeaderRecord!.Select(Normalize).ToArray();

                    while (csv.Read())
                    {
                        var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 0; i < headers.Length; i++)
                        {
                            string h = headers[i];
                            string v = csv.TryGetField(i, out string? value) ? (value ?? "") : "";
                            row[h] = v.Trim();
                        }
                        yield return row;
                    }

                    yield break; // si llegó aquí, no probamos el siguiente encoding
                }
                finally
                {
                    // probar siguiente encoding
                }
            }
        }

        private static string DetectDelimiter(string headerLine)
        {
            // Contamos ocurrencias de delimitadores comunes
            var candidates = new[] { "\t", "|", ";", "," };
            string best = "\t"; // preferimos TAB (caso típico en TXT)
            int bestCount = -1;

            foreach (var d in candidates)
            {
                int count = headerLine.Split(new[] { d }, StringSplitOptions.None).Length - 1;
                if (count > bestCount)
                {
                    bestCount = count;
                    best = d;
                }
            }
            return best;
        }

        // =================== Helpers ===================
        private static string Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var x = s.Trim().ToUpperInvariant().Replace(" ", "");
            x = x.Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U").Replace("Ñ", "N");
            return x;
        }

        private static string? Get(Dictionary<string, string>? row, string columnName)
        {
            if (row == null) return null;
            var key = Normalize(columnName);
            return row.TryGetValue(key, out var v) ? v : null;
        }
    }
}
