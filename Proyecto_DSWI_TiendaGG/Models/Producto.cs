using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_DSWI_TiendaGG.Models
{
    public class Producto
    {
        [Display(Name ="Imagen")]public string img_pro { get; set; }
        [Display(Name="Codigo")]public string cod_pro { get; set; }
        [Display(Name = "Descripcion")] [Required] [RegularExpression(@"[a-zA-Z0-9\s]{1,50}")] public string desc_pro { get; set; }
        [Display(Name = "Precio")] [Required] public decimal pre_pro { get; set; }
        [Display(Name = "Stock")] [Required] [Range(1,999)] public int stock_pro { get; set; }
        [Display(Name = "Categoria")] public string cod_cat { get; set; }
        [Display(Name = "Estado")] public int est_pro { get; set; }
    }
}