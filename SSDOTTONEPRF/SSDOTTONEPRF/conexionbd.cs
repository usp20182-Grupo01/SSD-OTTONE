using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace SSDOTTONEPRF
{
    class conexionbd
    {
        string cadena = "Data Source=localhost;Initial Catalog=ssdprueba;User Id=ale; Password=1234;Integrated Security=True";
        public SqlConnection conectarbd = new SqlConnection();
        public conexionbd()
        {
            //conectarbd.ConnectionString = cadena;
            conectarbd.ConnectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

        }

        public void abrir()
        {
            try
            {
                conectarbd.Open();
                Console.WriteLine("Conexion Abierta");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error al abrir la BD "+ ex.Message);
            }
        }
        public void cerrar()
        {
            conectarbd.Close();
        }
    }
}
