using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using Proyecto_DSWI_TiendaGG.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using iText.IO.Image;
using iText.Layout.Element;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace Proyecto_DSWI_TiendaGG.Modulo
{
    public class BoletaDAO
    {

        public IEnumerable<Boleta> boletas()
        {
            List<Boleta> temporal = new List<Boleta>();
            using (SqlConnection cn = new SqlConnection(
                  ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from tb_CabBoleta",cn);
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Boleta b = new Boleta()
                    {
                        cod_bol = dr.GetString(0),
                        fec_bol = dr.GetDateTime(1),
                        cod_cli = dr.GetString(2),
                        tot_bol = dr.GetDecimal(3)
                    };
                    temporal.Add(b);
                }
                dr.Close(); cn.Close();
            }
            return temporal;
        }

        public Boleta Buscar(string codbol)
        {
            return boletas().Where(b => b.cod_bol == codbol).FirstOrDefault();
        }

        public IEnumerable<Detalle> Detalle(string cod)
        {
            List<Detalle> temporal = new List<Detalle>();

            using (SqlConnection cn = new SqlConnection(
                  ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from tb_DetBoleta where cod_bol = @cod",cn);
                cmd.Parameters.AddWithValue("@cod",cod);
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Detalle d = new Detalle()
                    {
                        cod_bol = dr.GetString(0),
                        cod_prod = dr.GetString(1),
                        desc_det = dr.GetString(2),
                        can_pro = dr.GetInt32(3),
                        pre_pro = dr.GetDecimal(4),
                    };
                    temporal.Add(d);
                }
                dr.Close(); cn.Close();
            }

            return temporal;
        }

        public string Autogenera()
        {
            string cod = null;

            using (SqlConnection cn = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select dbo.fx_AutogenerarBoleta()",cn);
                cod = (string)cmd.ExecuteScalar();
                cn.Close();
            }
            return cod;
        }

        public string Transaccion(Boleta b, List<Detalle> carrito, string para)
        {
            string msg = "";
            string codbol = Autogenera();

            using (SqlConnection cn = new SqlConnection(
                  ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                cn.Open();
                SqlTransaction tr = cn.BeginTransaction(IsolationLevel.Serializable);
                try { 
                    
                        SqlCommand cmd = new SqlCommand("insert tb_CabBoleta(cod_bol,fec_bol,cod_cli,tot_bol)" +
                            " values(@cod,GETDATE(),@cli,@tot)", cn, tr);
                        cmd.Parameters.AddWithValue("@cod", codbol);
                        cmd.Parameters.AddWithValue("@cli", b.cod_cli);
                        cmd.Parameters.AddWithValue("@tot", b.tot_bol);
                        cmd.ExecuteNonQuery();

                        foreach (Detalle d in carrito)
                        {
                            cmd = new SqlCommand("insert tb_DetBoleta " +
                                "values(@cod,@pro,@des,@can,@pre)", cn, tr);
                            cmd.Parameters.AddWithValue("@cod", codbol);
                            cmd.Parameters.AddWithValue("@pro", d.cod_prod);
                            cmd.Parameters.AddWithValue("@des", d.desc_det);
                            cmd.Parameters.AddWithValue("@can", d.can_pro);
                            cmd.Parameters.AddWithValue("@pre", d.pre_pro);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (Detalle d in carrito)
                        {
                            cmd = new SqlCommand("update tb_Producto set stock_pro-=@can where cod_pro = @pro", cn, tr);
                            cmd.Parameters.AddWithValue("@pro", d.cod_prod);
                            cmd.Parameters.AddWithValue("@can", d.can_pro);
                            cmd.ExecuteNonQuery();
                        }
                    tr.Commit();



                    msg = "Compra Exitosa : "+codbol +"\n"+"\n"+GenerarBoletaPDF(codbol,para);
                   }
                catch(SqlException ex)
                {
                    msg = ex.Message;
                    tr.Rollback();
                }
                finally {cn.Close();}
            }
            return msg;
        }

        public string GenerarBoletaPDF(string cod,string para)
        {
            string msg = "";

            Boleta b = Buscar(cod);

            List<Detalle> detalle = (List<Detalle>)Detalle(cod);

            string ruta = HttpContext.Current.Server.MapPath("~/boleta/"+cod+".pdf");

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
                    doc.Add(new Paragraph("Boleta N° "+b.cod_bol).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph("Cod. Cliente : " + b.cod_cli));
                    doc.Add(new Paragraph("Fecha : " + b.fec_bol.ToShortDateString()));
                    doc.Add(new Paragraph("Total pagado : " + b.tot_bol));
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph(" "));
                    doc.Add(new Paragraph("Detalle de Compra"));
                    Table t = new Table(4);
                    
                    t.AddCell("ID");
                    t.AddCell("Producto");
                    t.AddCell("Precio");
                    t.AddCell("Cantidad");
                    foreach (var d in detalle)
                    {
                        t.AddCell(d.cod_prod);
                        t.AddCell(d.desc_det);
                        t.AddCell(d.pre_pro.ToString());
                        t.AddCell(d.can_pro.ToString());
                    }

                    doc.Add(t);
                    doc.Close();

                    msg = "Boleta generada exitosamente y "+EnviarPDF(ruta,para);

                }
                catch(Exception ex) { msg = ex.Message; }
            }

                return msg;
        }

        public string EnviarPDF(string ruta, string para)
        {
            MailMessage mmsg = new MailMessage();
            mmsg.To.Add(para);
            mmsg.Subject = "Boleta de Venta";
            mmsg.SubjectEncoding = Encoding.UTF8;

            string mensaje = "Buen dia," + "\n" + "\n" + "Le hacemos presente la Boleta de Venta en PDF." + "\n" + "\n" + "Saludos.";

            mmsg.Attachments.Add(new Attachment(@ruta));
            mmsg.Body = mensaje;
            mmsg.BodyEncoding = Encoding.UTF8;
            mmsg.From = new MailAddress("tiendagg.facturacion@gmail.com");

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("tiendagg.facturacion@gmail.com", "Tgg135799");

            string output;

            try
            {
                smtp.Send(mmsg);
                mmsg.Dispose();
                output = "fue enviado a su correo";
            }
            catch (Exception ex) { output = "Error al enviar correo : " + ex.Message; }

            return output;
        }
       
    }
}