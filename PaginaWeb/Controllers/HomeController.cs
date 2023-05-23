using PaginaWeb.Models;
using PaginaWeb.Permisos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaginaWeb.Controllers
{
    [ValidarSesion]
    public class HomeController : Controller
    {
        //static string cadena = "Data source=DESKTOP-P8B0I4F\\SLQ2022;Initial Catalog = DBPRUEBA; User Id=Anyi; Password=1234; Integrated Security=true ";
        static string cadena = "Server=tcp:pruebaserve.database.windows.net,1433;Initial Catalog=DBPRUEBA;Persist Security Info=False;User ID=adminprueba;Password=4dm1n15pr#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

		public ActionResult Index()
        {
            return View();
        }

        public ActionResult About(Alumno oAlumno)
        {
            TablaViewModel tablaViewModels = new TablaViewModel();
            tablaViewModels.Tabla2 = new List<PaginaWeb.Models.Alumno>();
            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM ALUMNO WHERE Identificacion LIKE '%" + oAlumno.Identificacion + "%'", connection);
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
            return View(tablaViewModels);
        }

        public ActionResult Contact(Curso oCurso)
        {
            TablaViewModel tablaViewModels = new TablaViewModel();
            tablaViewModels.Tabla3 = new List<PaginaWeb.Models.Curso>();
            using (SqlConnection connection = new SqlConnection(cadena))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM CURSO WHERE NombreCurso LIKE '%" + oCurso.NombreCurso + "%'", connection);
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
        public ActionResult CerrarSesion()
        {

            Session["usuario"] = null;
            return RedirectToAction("Login", "Acceso");
        }
        [ValidarSesion]
        public ActionResult mostrarinfo()
        {

            return View();
        }
        [HttpPost]
        public ActionResult EliminarAlumno(Alumno oAlumno)
        {
            // Llamar al procedimiento almacenado utilizando ADO.NET
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                // Llamar al procedimiento almacenado de eliminación en la base de datos
                using (SqlCommand cmd = new SqlCommand("DELETE FROM ALUMNO WHERE IdAlumno = @IdAlumno", cn))
                {
                    cmd.Parameters.AddWithValue("@IdAlumno", oAlumno.IdAlumno);
                    int result = cmd.ExecuteNonQuery();

                    // Verificar si la eliminación fue exitosa
                    if (oAlumno.IdAlumno > 0)
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
            return RedirectToAction("About", "Home");
        }

        [HttpPost]
        public ActionResult EliminarCurso(Curso oCurso)
        {
            // Llamar al procedimiento almacenado utilizando ADO.NET
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                // Llamar al procedimiento almacenado de eliminación en la base de datos
                using (SqlCommand cmd = new SqlCommand("DELETE FROM CURSO WHERE IdCurso = @IdCurso", cn))
                {
                    cmd.Parameters.AddWithValue("@IdCurso", oCurso.IdCurso);
                    int result = cmd.ExecuteNonQuery();

                    // Verificar si la eliminación fue exitosa
                    if (oCurso.IdCurso > 0)
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
            return RedirectToAction("About", "Home");
        }
      
       

    }
}