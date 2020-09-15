using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_DSWI_TiendaGG.Models
{
    public class Producto
    {
        [Display(Name="Codigo")]public string cod_pro { get; set; }
        [Display(Name = "Descripcion")] public string desc_pro { get; set; }
        [Display(Name = "Precio")] public decimal pre_pro { get; set; }
        [Display(Name = "Stock")] public int stock_pro { get; set; }
        [Display(Name = "Categoria")] public string cod_cat { get; set; }
        [Display(Name = "Estado")] public int est_pro { get; set; }
    }
}