using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_DSWI_TiendaGG.Models
{
    public class CabRep
    {
        public int año { get; set; }
        public int mes { get; set; }
        public int ventasTot { get; set; }
        public decimal acumuladoTot { get; set; }
    }
}