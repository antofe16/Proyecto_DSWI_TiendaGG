using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Proyecto_DSWI_TiendaGG.Models;
using Proyecto_DSWI_TiendaGG.Modulo;
using System.Data.SqlClient;
using System.Web.WebPages;

namespace Proyecto_DSWI_TiendaGG.Controllers
{
    public class NegociosController : Controller
    {

        ProductoDAO miproducto = new ProductoDAO();
        BoletaDAO miboleta = new BoletaDAO();
        ClienteDAO micliente = new ClienteDAO();
        AdminDAO miadmin = new AdminDAO();

        //Vista de Administrador
        public ActionResult LoginAdmin(string mensaje = "")
        {
            ViewBag.mensaje = mensaje;
            ViewBag.tipo = "LoginAdm";
            return View();
        }
        [HttpPost]
        public ActionResult LoginAdmin(string usu = null, string cla = null)
        {
            string msg;

            Administrador reg = miadmin.Login(usu, cla);

            if (reg.cod_adm != null)
            {
                Session["Admin"] = reg;
                return RedirectToAction("InicioAdmin");
            }
            else
            {
                msg = "Usuario o clave incorrectos";
                return RedirectToAction("LoginAdmin", new { mensaje = msg });
            }
        }

        public ActionResult InicioAdmin()
        {
            ViewBag.tipo = "Admin";
            return View();
        }
        
        public ActionResult ConsultaProductos(int p = 0, string op = "", string idcat = "", string term = "", decimal p1 = 0, decimal p2 = 0)
        {
            ViewBag.categorias = new SelectList(miproducto.categorias(),"cod_cat","nom_cat");
            ViewBag.cat = miproducto.categorias();
            ViewBag.tipo = "Admin";
            IEnumerable<Producto> temporal;

            List<SqlParameter> ps = new List<SqlParameter>
            {
                new SqlParameter("@term",term),
                new SqlParameter("@idcat",idcat),
                new SqlParameter("@p1",p1),
                new SqlParameter("@p2",p2)
            };

            temporal = miproducto.consultaMultiple(ps);
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

        public ActionResult CreateProducto()
        {
            ViewBag.categorias = new SelectList(miproducto.categorias(), "cod_cat", "nom_cat");
            ViewBag.tipo = "Admin";
            return View(new Producto());
        }

        [HttpPost] public ActionResult CreateProducto(Producto reg, HttpPostedFileBase archivo)
        {
            ViewBag.tipo = "Admin";

            ViewBag.categorias = new SelectList(miproducto.categorias(), "cod_cat", "nom_cat", reg.cod_cat);

            if (!ModelState.IsValid) { ViewBag.mensaje = "Corrija los errores"; return View(reg); }

            if (archivo == null) { ViewBag.mensaje = "Adjunte una imagen"; return View(reg);}

            List<SqlParameter> parametros = new List<SqlParameter>() {
                new SqlParameter("@desc",reg.desc_pro),
                new SqlParameter("@pre",reg.pre_pro),
                new SqlParameter("@stk",reg.stock_pro),
                new SqlParameter("@idcat",reg.cod_cat),
                new SqlParameter("@img","~/img/prod/"+System.IO.Path.GetFileName(archivo.FileName))
            };

            ViewBag.mensaje = miproducto.CRUD("usp_insert_producto", parametros, 1);

            archivo.SaveAs(System.IO.Path.Combine(
                    Server.MapPath("~/img/prod"), System.IO.Path.GetFileName(archivo.FileName)));

            return View(reg);
        }

        public ActionResult EditProducto(string cod)
        {
            if (cod == null) return RedirectToAction("ConsultaProductos");
            Producto reg = miproducto.Buscar(cod);

            ViewBag.categorias = new SelectList(miproducto.categorias(), "cod_cat", "nom_cat", reg.cod_cat);
            ViewBag.tipo = "Admin";

            return View(reg);
        }

        [HttpPost] public ActionResult EditProducto(Producto reg, HttpPostedFileBase archivo)
        {
            ViewBag.tipo = "Admin";
            ViewBag.categorias = new SelectList(miproducto.categorias(), "cod_cat", "nom_cat", reg.cod_cat);

            if (!ModelState.IsValid) { ViewBag.mensaje = "Corrija los errores"; return View(); }
            
            if (archivo == null)
            {
                Producto p = miproducto.Buscar(reg.cod_pro);

                List<SqlParameter> parametros = new List<SqlParameter>() {
                new SqlParameter("@cod",reg.cod_pro),
                new SqlParameter("@desc",reg.desc_pro),
                new SqlParameter("@pre",reg.pre_pro),
                new SqlParameter("@stk",reg.stock_pro),
                new SqlParameter("@idcat",reg.cod_cat),
                new SqlParameter("@img",p.img_pro)
                };

                ViewBag.mensaje = miproducto.CRUD("usp_update_producto", parametros, 2);
               
            }

            else
            {
                List<SqlParameter> parametros = new List<SqlParameter>() {
                new SqlParameter("@cod",reg.cod_pro),
                new SqlParameter("@desc",reg.desc_pro),
                new SqlParameter("@pre",reg.pre_pro),
                new SqlParameter("@stk",reg.stock_pro),
                new SqlParameter("@idcat",reg.cod_cat),
                new SqlParameter("@img","~/img/prod/"+System.IO.Path.GetFileName(archivo.FileName))
                };

                ViewBag.mensaje = miproducto.CRUD("usp_update_producto", parametros, 2);

                archivo.SaveAs(System.IO.Path.Combine(
                        Server.MapPath("~/img/prod/"), System.IO.Path.GetFileName(archivo.FileName)));
            }

            return View();
        }

        public ActionResult DescontinuarProducto(string cod = "")
        {
            ViewBag.tipo = "Admin";
            ViewBag.mensaje = miproducto.Descontinuar(cod);
            return View();
        }

        public ActionResult Detalles(string cod)
        {
            ViewBag.tipo = "Admin";
            if (cod == null) return RedirectToAction("ConsultaProductos");
            
            return View(miproducto.Buscar(cod));
        }

        public ActionResult SoporteAdmin()
        {
            ViewBag.tipo = "Admin";
            return View();
        }

        [HttpPost]
        public ActionResult SoporteAdmin(string nombre, string apellidos, string telefono, string email, string mensaje)
        {
            ViewBag.tipo = "Admin";

            string msg = "Nombres: " + nombre + "\n" + "\n" +
                         "Apellidos: " + apellidos + "\n" + "\n" +
                         "Telefono: " + telefono + "\n" + "\n" +
                         "Email: " + email + "\n" + "\n" +
                         "\n" + "Mensaje:" + "\n" +
                         mensaje;
            string rs = miadmin.Soporte(msg).ToUpper();

            ViewBag.mensaje = rs;

            return View();
        }

        public ActionResult Reportes(int? mes = 0, int? año = 0, string msg = "" )
        {
            ViewBag.tipo = "Admin";
            ViewBag.msg = msg;

            List<RepDet> detalle = (List<RepDet>)miadmin.Detalle(mes, año);

            ViewBag.detalle = detalle;
            ViewBag.reporte = miadmin.Buscar(mes,año);

            return View(miadmin.meses());
        }

        public ActionResult GenerarPDF(int mes, int año)
        {
            ViewBag.tipo = "Admin";

            Administrador a = (Administrador)Session["Admin"];

            string msg = miadmin.ReportePDF(año,mes,a.email_adm.ToString());

            return RedirectToAction("Reportes",new { msg = msg});
        }

        public ActionResult CerrarSesionAdm()
        {
            Session.Abandon();
            return RedirectToAction("LoginAdmin");
        }
        //------------------------------------------------------------//

        //Vista de Cliente
        public ActionResult Index()
        {
            ViewBag.tipo = "Index";
            return View();
        }

        public ActionResult Registro()
        {
            ViewBag.tipo = "LoginCli";
            return View(new Cliente());
        }

        [HttpPost]public ActionResult Registro(Cliente reg)
        {
            ViewBag.tipo = "LoginCli";
            ViewBag.mensaje = micliente.Registrar(reg);

            return View();
        }

        public ActionResult LoginCliente(string mensaje = "")
        {
            if (Session["carrito"] == null)
            {
                Session["carrito"] = new List<Detalle>();
            }
            ViewBag.mensaje = mensaje;
            ViewBag.tipo = "LoginCli";
            return View();
        }

        [HttpPost]public ActionResult LoginCliente(string usu= null, string cla = null)
        {
            string msg;

            Cliente reg = micliente.Login(usu,cla);

            if (reg.cod_cli != null)
            {
                Session["cliente"] = reg;
                return RedirectToAction("InicioCliente");
            }
            else
            {
                msg = "Usuario o clave incorrectos";
                return RedirectToAction("LoginCliente", new { mensaje = msg });
            }
        }
        public ActionResult InicioCliente()
        {
            ViewBag.tipo = "Cliente";

            Cliente c = (Cliente)Session["cliente"];

            ViewBag.datos = c.nom_cli+" "+c.ape_cli;
            ViewBag.email = c.email_cli;

            return View();
        }

        public ActionResult VistaProductos(int p = 0, string op = "", string idcat = "", string term = "", double p1 = 0, double p2 = 0)
        {
            ViewBag.categorias = new SelectList(miproducto.categorias(), "cod_cat", "nom_cat");
            ViewBag.tipo = "Cliente";
            IEnumerable<Producto> temporal;

            List<SqlParameter> ps = new List<SqlParameter>
            {
                new SqlParameter("@term",term),
                new SqlParameter("@idcat",idcat),
                new SqlParameter("@p1",p1),
                new SqlParameter("@p2",p2)
            };

            temporal = miproducto.consultaMultiple(ps);

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
            return View(temporal.Skip(p * nr).Take(nr));
        }

        public ActionResult Select(string cod = "", string mensaje = "")
        {
            ViewBag.tipo = "Cliente";

            if (cod == "") return RedirectToAction("VistaProductos");

            ViewBag.mensaje = mensaje;
            return View(miproducto.Buscar(cod));
        }

        public ActionResult Registra(string cod, int cantidad)
        {

            Producto reg = miproducto.Buscar(cod);

            if (cantidad <= 0 || cantidad > reg.stock_pro)
                return RedirectToAction("Select", new { mensaje = "Cantidad minima 1 hasta el stock", cod = reg.cod_pro });

            Detalle d = new Detalle();
            d.cod_prod = reg.cod_pro;
            d.desc_det = reg.desc_pro;
            d.pre_pro = reg.pre_pro;
            d.can_pro = cantidad;
            d.img_det = reg.img_pro;

            List<Detalle> temporal = (List<Detalle>)Session["carrito"];
            temporal.Add(d);

            return RedirectToAction("Select", new { mensaje = "Producto : "+ reg.desc_pro +" agregado al carrito", cod = reg.cod_pro });
        }

        public ActionResult Comprar(int p = 0, string op= "")
        {
            ViewBag.tipo = "Cliente";

            List<Detalle> temporal = (List<Detalle>)Session["carrito"];

            int nr = 3;
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

            ViewBag.total = temporal.Sum(d => d.pre_pro * d.can_pro);
            ViewBag.pags = pags;
            ViewBag.p = p;
            ViewBag.c = c;
            return View(temporal.Skip(p * nr).Take(nr));
        }

        public ActionResult Quitar(string cod = "")
        {
            List<Detalle> temporal = (List<Detalle>)Session["carrito"];
            foreach (Detalle it in temporal)
            {
                if (it.cod_prod == cod)
                {
                    temporal.Remove(it); break;
                }
            }

            return RedirectToAction("Comprar");
        }

        public ActionResult Pago(string mensaje)
        {
            ViewBag.tipo = "Cliente";

            Cliente c = (Cliente)Session["cliente"];

            ViewBag.cli = c.cod_cli;
            
            string fecha = DateTime.Today.ToShortDateString();

            decimal total = 0;

            List<Detalle> temporal = (List<Detalle>)Session["carrito"];
            foreach (Detalle d in temporal)
            {
                
                total += d.pre_pro * d.can_pro;
            }

            ViewBag.fecha = fecha;
            ViewBag.total = total;

            return View(new Boleta());
        }

        [HttpPost]public ActionResult Pago(Boleta reg)
        {
            ViewBag.tipo = "Cliente";
            string msg;

            decimal total = 0;

            List<Detalle> temporal = (List<Detalle>)Session["carrito"];

            if (temporal.Count() == 0) { ViewBag.msg = "Su carrito esta vacio"; return View(); }

            Cliente c = (Cliente)Session["cliente"];

            foreach (Detalle d in temporal)
            {
                total += d.pre_pro * d.can_pro;
            }

            reg.cod_cli = c.cod_cli;
            reg.tot_bol = total;
      
            msg = miboleta.Transaccion(reg, temporal,c.email_cli);

            Session["carrito"] = new List<Detalle>();

            ViewBag.msg = msg;

            return View("Pago");
        }

        public ActionResult SoporteCliente()
        {
            ViewBag.tipo = "Cliente";
            return View();
        }

        [HttpPost] public ActionResult SoporteCliente(string nombre, string apellidos, string telefono, string email, string mensaje)
        {
            ViewBag.tipo = "Cliente";

            string msg = "Nombres: " + nombre + "\n" + "\n" +
                         "Apellidos: " + apellidos + "\n" + "\n" +
                         "Telefono: " + telefono + "\n" + "\n" +
                         "Email: " + email + "\n" + "\n" +
                         "\n" + "Mensaje:" + "\n" +
                         mensaje;
            string rs = micliente.Soporte(msg).ToUpper();

            ViewBag.mensaje = rs;

            return View();
        }
        
        [HttpPost]public ActionResult InicioCliente(string email)
        {
            ViewBag.tipo = "Cliente";
            string rs = micliente.Catalogo(email,Server.MapPath("~/img/catalogo/catalogo.pdf"));
            ViewBag.mensaje = rs;

            return View();
        }
        
        public ActionResult CerrarSesionCli()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }
        //------------------------------------------------------------//
    }
}