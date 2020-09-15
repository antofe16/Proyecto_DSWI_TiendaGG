using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_DSWI_TiendaGG.Models;
using Proyecto_DSWI_TiendaGG.Modulo;

namespace Proyecto_DSWI_TiendaGG.Controllers
{
    public class NegociosController : Controller
    {

        ProductoDAO miproducto = new ProductoDAO();

        public ActionResult InicioAdmin()
        {
            ViewBag.tipo = "Admin";
            return View();
        }
        
        public ActionResult Index(int p = 0, string op = "", string idcat = "", string term = "")
        {
            ViewBag.categorias = new SelectList(miproducto.categorias(),"cod_cat","nom_cat");
            ViewBag.tipo = "Admin";
            IEnumerable<Producto> temporal = null;

            if(idcat == "C00" && term == null) { temporal = miproducto.productos(); }

            else if (idcat == "C00" && term != null) { temporal = miproducto.productos().Where(pr=>pr.desc_pro.ToUpper().Contains(term.ToUpper())); }

            else if (idcat == "" && term == "") { temporal = new List<Producto>(); }
          
            else if (idcat != "C00" && term != null) { temporal = miproducto.productos().Where(pr => pr.cod_cat == idcat && pr.desc_pro.ToUpper().Contains(term.ToUpper())); }

            int nr = 8;
            int c = temporal.Count(); 
            int pags = c % nr > 0 ? c / nr + 1 : c / nr;

            if (op == "+")
            {
                p++;
                if (p >= pags - 1) p = pags - 1;
            }
            else if (op == "-")
            {
                p--;
                if (p <= 0) p = 0;
            }

            ViewBag.pags = pags;
            ViewBag.p = p;
            ViewBag.term = term;
            ViewBag.idcat = idcat;
            ViewBag.c = c;
            return View(temporal.Skip(p*nr).Take(nr));
        }

        public ActionResult Detalles(string cod = null)
        {
            ViewBag.tipo = "Admin";
            if (cod == null) return RedirectToAction("Index");

            return View(miproducto.productos().Where(p => p.cod_pro == cod).FirstOrDefault());
        }

        public ActionResult InicioCliente()
        {
            ViewBag.tipo = "Cliente";
            return View();
        }

        
    }
}