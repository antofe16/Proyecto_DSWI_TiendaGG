using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_DSWI_TiendaGG.Models
{
    public class Cliente
    {
        public string cod_cli { get; set; }
        [Display(Name ="Nombres")][Required][RegularExpression(@"[a-zA-Z0-9\s]{1,50}")] public string nom_cli { get; set; }
        [Display(Name = "Apellidos")] [Required] [RegularExpression(@"[a-zA-Z0-9\s]{1,50}")] public string ape_cli { get; set; }
        [Display(Name = "Direccion")] [Required] [RegularExpression(@"[a-zA-Z0-9.\s]{1,50}")] public string dir_cli { get; set; }
        [Display(Name = "Email")] [Required] [DataType(DataType.EmailAddress)] public string email_cli { get; set; }
        [Display(Name = "DNI")] [Required] [RegularExpression(@"[0-9]{8}")] public string dni_cli { get; set; }
        [Display(Name = "Telefono")] [Required] [RegularExpression(@"[0-9]{6,15}")] public string tel_cli { get; set; }
        [Display(Name = "Usuario")] [Required] [MinLength(3)][MaxLength(15)] public string usu_cli { get; set; }
        [Display(Name = "Contraseña")] [Required] [MinLength(8)] [MaxLength(15)] public string cla_cli { get; set; }
        public int est_cli { get; set; }
    }
}