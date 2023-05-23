using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaginaWeb.Models
{
    public class Curso
    {
        public int IdCurso { get; set; }
        public int Codigo { get; set; }
        public string NombreCurso { get; set; }
        public int Creditos { get; set; }
        public int Id_Usuario { get; set; }

    }
}