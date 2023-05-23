using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaginaWeb.Models
{
    public class TablaViewModel
    {
        public List<PaginaWeb.Models.Usuario> Tabla1 { get; set; }
        public List<PaginaWeb.Models.Alumno> Tabla2 { get; set; }
        public List<PaginaWeb.Models.Curso> Tabla3 { get; set; }
        public string FiltroTabla1 { get; set; }
        public string FiltroTabla2 { get; set; }
        public string FiltroTabla3 { get; set; }
    }
}