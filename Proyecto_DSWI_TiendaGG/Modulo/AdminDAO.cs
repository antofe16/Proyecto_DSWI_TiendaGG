using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Proyecto_DSWI_TiendaGG.Models;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Net;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Geom;
using iText.IO.Image;
using iText.Layout.Element;

namespace Proyecto_DSWI_TiendaGG.Modulo
{
    public class AdminDAO
    {
        public Administrador Login(string usu, string cla)
        {
            Administrador admin = new Administrador();
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_login_admin", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user", usu);
                cmd.Parameters.AddWithValue("@pass", cla);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    admin.cod_adm = dr.GetString(0);
                    admin.nom_adm = dr.GetString(1);
                    admin.ape_adm = dr.GetString(2);
                    admin.dir_adm = dr.GetString(3);
                    admin.email_adm = dr.GetString(4);
                    admin.tel_adm = dr.GetString(5);
                    admin.usu_adm = dr.GetString(6);
                    admin.cla_adm = dr.GetString(7);
                    admin.est_adm = dr.GetInt32(8);
                }
                dr.Close(); cn.Close();
            }

            return admin;
        }

        public string Soporte(string mensaje)
        {
            MailMessage mmsg = new MailMessage();
            mmsg.To.Add("antonio_felixcuya@hotmail.com");
            mmsg.Subject = "Solicitud de soporte";
            mmsg.SubjectEncoding = Encoding.UTF8;

            mmsg.Body = mensaje;
            mmsg.BodyEncoding = Encoding.UTF8;
            mmsg.From = new MailAddress("tiendagg.mensajero@gmail.com ");

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("tiendagg.mensajero@gmail.com ", "Tgg246810");

            string output;

            try
            {
                smtp.Send(mmsg);
                mmsg.Dispose();
                output = "La solicitud fue enviada exitosamente";
            }
            catch (Exception ex) { output = "Error al enviar correo : " + ex.Message; }

            return output;
        }

        public string ReportePDF(int año , int mes, string para)
        {
            string msg = "";

            string[] meses = { "Nada :v", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

            CabRep cabecera = Buscar(mes,año);
            List<RepDet> detalle = (List<RepDet>)Detalle(mes, año);

            string ruta = HttpContext.Current.Server.MapPath("~/pdf/reporte.pdf");

            using (PdfWriter writer = new PdfWriter(ruta))
            using (PdfDocument pdfdoc = new PdfDocument(writer))
            using (Document doc = new Document(pdfdoc, PageSize.LETTER))

            {
                try
                {
                    string url = HttpContext.Current.Server.MapPath("~/img/TiendaGG-logo.png");
                    Image img = new Image(ImageDataFactory.Create(url));
                    img.SetHeight(50);
                    img.SetWidth(50);
                    doc.Add(img);
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph("Reporte de Ventas por Mes").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph("Mes - Año : " + meses[cabecera.mes] + "-" + cabecera.año));
                    doc.Add(new Paragraph("Total de ventas : " + cabecera.ventasTot));
                    doc.Add(new Paragraph("Total Acumulado : " + cabecera.acumuladoTot));
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph(" "));
                    Table tabla = new Table(2);
                    tabla.AddCell("Producto");
                    tabla.AddCell("Cantidad Vendida");
                    foreach (var d in detalle)
                    {
                        tabla.AddCell(d.producto);
                        tabla.AddCell(d.cantidadVen.ToString());
                    }
                    tabla.AddCell("Total");
                    tabla.AddCell(detalle.Sum(d => d.cantidadVen).ToString());
                  
                    doc.Add(tabla);
                    doc.Close();   

                    msg = "PDF generado exitosamente y "+EnviarPDF(para,ruta);

                }
                catch (Exception ex) { msg = ex.Message; }

            }

            return msg;
        }

        public string EnviarPDF(string para, string ruta)
        {
            MailMessage mmsg = new MailMessage();
            mmsg.To.Add(para);
            mmsg.Subject = "Reporte de Ventas";
            mmsg.SubjectEncoding = Encoding.UTF8;

            string mensaje = "Buen dia," + "\n" + "\n" + "Le hacemos presente el reporte PDF que usted genero" + "\n" + "\n" + "Saludos.";

            mmsg.Attachments.Add(new Attachment(@ruta));
            mmsg.Body = mensaje;
            mmsg.BodyEncoding = Encoding.UTF8;
            mmsg.From = new MailAddress("tiendagg.mensajero@gmail.com");

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("tiendagg.mensajero@gmail.com ", "Tgg246810");

            string output;

            try
            {
                smtp.Send(mmsg);
                mmsg.Dispose();
                output = "fue enviado exitosamente a su correo";
            }
            catch (Exception ex) { output = "Error al enviar correo : " + ex.Message; }

            return output;
        }

        public IEnumerable<CabRep> meses()
        {
            List<CabRep> temporal = new List<CabRep>();
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select Month(fec_bol),Year(fec_bol) , count(cod_bol), sum(tot_bol)" +
                    " from tb_CabBoleta " +
                    "group by fec_bol", cn);

                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    CabRep mes = new CabRep()
                    {
                        mes = dr.GetInt32(0),
                        año = dr.GetInt32(1),
                        ventasTot = dr.GetInt32(2),
                        acumuladoTot = dr.GetDecimal(3)
                    };
                    temporal.Add(mes);
                }
                dr.Close(); cn.Close(); 
            }

                return temporal;
        }

        public CabRep Buscar(int ? mes, int ? año)
        {
            return meses().Where(m => m.mes == mes && m.año == año).FirstOrDefault();
        }

        public IEnumerable<RepDet> Detalle(int ? mes, int ? año)
        {
            List<RepDet> temporal = new List<RepDet>();
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select Month(fec_bol),Year(fec_bol) , des_pro ,sum(can_pro)  from tb_DetBoleta d" +
                    " join tb_CabBoleta c on d.cod_bol=c.cod_bol where Month(fec_bol) = @mes and Year(fec_bol) = @año " +
                    "group by fec_bol,cod_pro , des_pro", cn);
                cmd.Parameters.AddWithValue("@mes",mes);
                cmd.Parameters.AddWithValue("@año", año);
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    RepDet d = new RepDet()
                    {
                        mes = dr.GetInt32(0),
                        año = dr.GetInt32(1),
                        producto = dr.GetString(2),
                        cantidadVen = dr.GetInt32(3)
                    };
                    temporal.Add(d);
                }
                dr.Close(); cn.Close();
            }
            return temporal;
        }
    }
}