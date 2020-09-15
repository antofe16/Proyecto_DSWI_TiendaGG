using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_DSWI_TiendaGG.Models
{
    public class Detalle
    {
        public string cod_bol { get; set; }
        public string cod_prod { get; set; }
        public string desc_det { get; set; }
        public string img_det { get; set; }
        [RegularExpression("")][Range(1,999)]public int can_pro { get; set; }
        public decimal pre_pro { get; set; }
    }
}