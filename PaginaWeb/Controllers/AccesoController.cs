using PaginaWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PaginaWeb.Permisos;
using System.Threading.Tasks;
using System.Collections;
using System.Web.UI.WebControls;
using Microsoft.Win32;
using System.Runtime.Remoting.Messaging;

namespace PaginaWeb.Controllers
{
    
    public class AccesoController : Controller
    {
		//static string cadena = "Data source=DESKTOP-P8B0I4F\\SLQ2022;Initial Catalog = DBPRUEBA; User Id=Anyi; Password=1234; Integrated Security=true ";
		static string cadena = "Server=tcp:pruebaserve.database.windows.net,1433;Initial Catalog=DBPRUEBA;Persist Security Info=False;User ID=adminprueba;Password=4dm1n15pr#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		// GET: Acceso
		public ActionResult Login()
        {
            return View();
        }
        public ActionResult Registrar()
        {
            return View();
        }
       
        [ValidarSesion]
        public ActionResult Usuario(Alumno oAlumno)
        {
            Usuario oUsuario = (Usuario)Session["usuario"];
            TablaViewModel tablaViewModels = new TablaViewModel();
            tablaViewModels.Tabla2 = new List<PaginaWeb.Models.Alumno>();
            tablaViewModels.Tabla3 = new List<PaginaWeb.Models.Curso>();
            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM ALUMNO WHERE Id_Usuario LIKE '%" + oUsuario.IdUsuario + "%'", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tablaViewModels.Tabla2.Add(new PaginaWeb.Models.Alumno
                    {
                        IdAlumno = (int)reader["IdAlumno"],
                        Identificacion = (int)reader["Identificacion"],
                        Nombre = reader["Nombre"].ToString(),
                        PApellido = reader["PApellido"].ToString(),
                        SApelldo = reader["SApelldo"].ToString()
                    });
                }
            }
            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM CURSO WHERE Id_Usuario LIKE '%" + oUsuario.IdUsuario + "%'", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tablaViewModels.Tabla3.Add(new PaginaWeb.Models.Curso
                    {
                        IdCurso = (int)reader["IdCurso"],
                        Codigo = (int)reader["Codigo"],
                        NombreCurso = reader["NombreCurso"].ToString(),
                        Creditos = (int)reader["Creditos"]
                    });
                }
            }
            return View(tablaViewModels);
        }
        [ValidarSesion]
        public ActionResult Admin(Usuario oUsuario)
        {
            TablaViewModel tablaViewModels = new TablaViewModel();
            tablaViewModels.Tabla1 = new List<PaginaWeb.Models.Usuario>();

            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
               // SqlCommand command = new SqlCommand("SELECT * FROM USUARIO", connection);
                SqlCommand command = new SqlCommand("SELECT * FROM USUARIO WHERE NombreCompleto LIKE '%" + oUsuario.NombreCompleto + "%'", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tablaViewModels.Tabla1.Add(new PaginaWeb.Models.Usuario
                    {
                        IdUsuario = (int)reader["IdUsuario"],
                        NombreCompleto = reader["NombreCompleto"].ToString(),
                        Correo = reader["Correo"].ToString(),
                        Login = reader["Login"].ToString(),
                        Clave = reader["Clave"].ToString()
                    });
                }
            }
            return View(tablaViewModels);
        }
        [HttpPost]
        public ActionResult Registrar(Usuario oUsuario, Alumno oAlumno, Curso oCurso)
        {
            bool registrado;
            String mensaje;

            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();
                using (SqlCommand cdm = new SqlCommand("InsertarUser", cn))
                {
                    cdm.CommandType = CommandType.StoredProcedure;
                    cdm.Parameters.AddWithValue("NombreCompleto", oUsuario.NombreCompleto);
                    cdm.Parameters.AddWithValue("Correo", oUsuario.Correo);
                    cdm.Parameters.AddWithValue("Login", oUsuario.Login);
                    cdm.Parameters.AddWithValue("Clave", oUsuario.Clave);

                    cdm.Parameters.AddWithValue("Identificacion", oAlumno.Identificacion);
                    cdm.Parameters.AddWithValue("Nombre", oAlumno.Nombre);
                    cdm.Parameters.AddWithValue("PApellido", oAlumno.PApellido);
                    cdm.Parameters.AddWithValue("SApelldo", oAlumno.SApelldo);

                    cdm.Parameters.AddWithValue("Codigo", oCurso.Codigo);
                    cdm.Parameters.AddWithValue("NombreCurso", oCurso.NombreCurso);
                    cdm.Parameters.AddWithValue("Creditos", oCurso.Creditos);

                    cdm.Parameters.Add("Registrado", SqlDbType.BigInt).Direction = ParameterDirection.Output;
                    cdm.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                    cdm.ExecuteNonQuery();

                    registrado = Convert.ToBoolean(cdm.Parameters["Registrado"].Value);
                    mensaje = cdm.Parameters["Mensaje"].Value.ToString();
                }

            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {

                return View();
            }

        }
        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cdm = new SqlCommand("sp_ValidarUsuario", cn);
                cdm.Parameters.AddWithValue("NombreCompleto", oUsuario.NombreCompleto);
                cdm.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cdm.Parameters.AddWithValue("Login", oUsuario.Login);
                cdm.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cdm.CommandType = CommandType.StoredProcedure;

                cn.Open();
                oUsuario.IdUsuario = Convert.ToInt32(cdm.ExecuteScalar());
            }
            if (oUsuario.IdUsuario == 1 )
            {
                Session["admin"] = oUsuario;
                return RedirectToAction("Admin","Acceso");
            }
            else if (oUsuario.IdUsuario != 0)
            {

                Session["usuario"] = oUsuario;
                return RedirectToAction("Usuario", "Acceso");

            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }

        }
        [HttpPost]
        public ActionResult EliminarDatos(Usuario oUsuario)
        {
            // Llamar al procedimiento almacenado utilizando ADO.NET
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                // Llamar al procedimiento almacenado de eliminación en la base de datos
                using (SqlCommand cmd = new SqlCommand("EliminarDatos", cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", oUsuario.IdUsuario);
                    int result = cmd.ExecuteNonQuery();

                    // Verificar si la eliminación fue exitosa
                    if (oUsuario.IdUsuario > 0)
                    {
                        TempData["Mensaje"] = "Registro eliminado exitosamente.";
                    }
                    else
                    {
                        TempData["Mensaje"] = "Error al eliminar el registro.";
                    }
                }
            }
            // Redirigir a la vista correspondiente después de la eliminación
            return RedirectToAction("Admin", "Acceso");
        }
        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
    }
}