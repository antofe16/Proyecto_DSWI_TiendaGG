using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_DSWI_TiendaGG.Models
{
    public class Boleta
    {
        public string cod_bol { get; set; }
        public DateTime fec_bol { get; set; }
        public string cod_cli { get; set; }
        public decimal tot_bol { get; set; }
    }
}