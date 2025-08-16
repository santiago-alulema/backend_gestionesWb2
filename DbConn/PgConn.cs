using Npgsql;
using System.Data;

namespace gestiones_backend.DbConn
{
    public class PgConn
    {
        public NpgsqlConnection connectSQL = new NpgsqlConnection();
        public string cadenaConnect = string.Empty;

        public void abrir()
        {
            try
            {
                //abrir conexion
                connectSQL.ConnectionString = cadenaConnect;
                connectSQL.Open();
            }
            catch (Exception ex)
            {
                this.guarda_errores(obtiene_fecha(), ex.Message);
                System.Environment.Exit(0);
            }
        }

        public void cerrar()
        {
            try
            {
                //cerrar conexion
                connectSQL.Close();
                connectSQL.Dispose();
            }
            catch (Exception ex)
            {
                this.guarda_errores(obtiene_fecha(), ex.Message);
                System.Environment.Exit(0);
            }
        }

        public DataTable ejecutarconsulta_dt(string consulta, int timeout = 0)
        {
            DataTable retorna = new DataTable();

            NpgsqlDataReader conn = default(NpgsqlDataReader);
            //Dim conn1 As OracleDataAdapter
            try
            {

                NpgsqlCommand comand = new NpgsqlCommand(consulta, connectSQL);
                if (timeout > 0)
                    comand.CommandTimeout = timeout;
                this.abrir();
                conn = comand.ExecuteReader();
                retorna.Load(conn);
            }
            catch (Exception ex)
            {
                this.guarda_errores(obtiene_fecha(), ex.Message);
                throw new Exception(ex.Message);
                //System.Environment.Exit(0);
            }
            this.cerrar();
            return retorna;
            //return null;
        }


        public string ejecutarconsulta_sin_dt(string consulta)
        {
            string resultado = string.Empty;
            DataTable retorna = new DataTable();
            NpgsqlCommand conn = default(NpgsqlCommand);
            try
            {
                this.abrir();
                conn = new NpgsqlCommand(consulta, connectSQL);
                conn.ExecuteNonQuery();
                resultado = "OK";
            }
            catch (Exception ex)
            {
                resultado = ex.Message;
                this.guarda_errores(obtiene_fecha(), ex.Message);
                //System.Environment.Exit(0);                
            }
            this.cerrar();
            return resultado;
        }

        public void ejecutaprocedimientos(string nombre, int timeout = 0)
        {
            try
            {
                this.abrir();

                using (var conn = new NpgsqlCommand(nombre, connectSQL))
                {
                    conn.CommandType = CommandType.StoredProcedure;
                    //conn.Parameters.AddWithValue("pfecha", NpgsqlDbType.Date, fecha);

                    if (timeout > 0)
                        conn.CommandTimeout = timeout;

                    conn.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                this.guarda_errores(obtiene_fecha(), ex.Message);
                //System.Environment.Exit(0);
            }
            finally
            {
                this.cerrar();
            }
        }


        public Boolean ejecutaprocedimientos_actualizarGcobranza_Imora(string[] _params)
        {
            return true;
        }

        private static readonly object lockObject = new object();

        public void guarda_errores(string fecha, string mensaje)
        {
            string consulta = Directory.GetCurrentDirectory();
            string rutaArchivo = Path.Combine(consulta, "error_log.txt");

            try
            {
                lock (lockObject)
                {
                    using (StreamWriter archivo = File.AppendText(rutaArchivo))
                    {
                        archivo.WriteLine($"{fecha} - - - - - - - {mensaje}");
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"No se pudo escribir en el archivo de log: {e.Message}");
            }
        }

        public static string obtiene_fecha()
        {
            string fechahora = Convert.ToString(DateTime.Now.ToLongDateString());
            return fechahora;
        }
    }

}