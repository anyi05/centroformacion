using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaginaWeb.Models
{
    public class Alumno
    {
        public int IdAlumno { get; set; }
        public int Identificacion { get; set; }
        public string Nombre { get; set; }
        public string PApellido { get; set; }
        public string SApelldo { get; set; }
        public int Id_Usuario { get; set; }
    }
}