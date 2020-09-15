using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_DSWI_TiendaGG.Models
{
    public class Cliente
    {
        public string cod_cli { get; set; }
        public string nom_cli { get; set; }
        public string ape_cli { get; set; }
        public string dir_cli { get; set; }
        public string email_cli { get; set; }
        public string dni_cli { get; set; }
        public string tel_cli { get; set; }
        public string usu_cli { get; set; }
        public string cla_cli { get; set; }
        public int est_cli { get; set; }
    }
}