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

namespace Proyecto_DSWI_TiendaGG.Modulo
{
    
    public class ClienteDAO
    {
        public Cliente Login(string usu, string cla)
        {
            Cliente client = new Cliente();
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_login_cliente", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@user", usu);
                cmd.Parameters.AddWithValue("@pass", cla);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    client.cod_cli = dr.GetString(0);
                    client.nom_cli = dr.GetString(1);
                    client.ape_cli = dr.GetString(2);
                    client.email_cli = dr.GetString(3);
                    client.dni_cli = dr.GetString(4);
                    client.tel_cli = dr.GetString(5);
                    client.dir_cli = dr.GetString(6);
                    client.usu_cli = dr.GetString(7);
                    client.cla_cli = dr.GetString(8);
                    client.est_cli = dr.GetInt32(9);
                }
                dr.Close(); cn.Close();
            }

            return client;
        }


        public string Registrar(Cliente reg)
        {
            string msg ="";
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("usp_create_cliente", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nom", reg.nom_cli);
                    cmd.Parameters.AddWithValue("@ape", reg.ape_cli);
                    cmd.Parameters.AddWithValue("@email", reg.email_cli);
                    cmd.Parameters.AddWithValue("@dni", reg.dni_cli);
                    cmd.Parameters.AddWithValue("@telf", reg.tel_cli);
                    cmd.Parameters.AddWithValue("@dir", reg.dir_cli);
                    cmd.Parameters.AddWithValue("@usu", reg.usu_cli);
                    cmd.Parameters.AddWithValue("@cla", reg.cla_cli);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    msg = "Cliente Registrado";
                }
                catch (SqlException ex) { msg = "Error al registrar - Error : "+ex.Number; }
                finally { cn.Close(); }
            }

                return msg;
        }

        public string Soporte(string mensaje)
        {
            MailMessage mmsg = new MailMessage();
            mmsg.To.Add("antonio_felixcuya@hotmail.com");
            mmsg.Subject = "Solicitud de soporte";
            mmsg.SubjectEncoding = Encoding.UTF8;
         
            mmsg.Body = mensaje;
            mmsg.BodyEncoding = Encoding.UTF8;
            mmsg.From = new MailAddress("tiendagg.mensajero@gmail.com");

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("tiendagg.mensajero@gmail.com", "Tgg246810");

            string output;

            try
            {
                smtp.Send(mmsg);
                mmsg.Dispose();
                output = "La solicitud fue enviada exitosamente";
            }
            catch(Exception ex) { output = "Error al enviar correo : " + ex.Message; }

            return output;
        }

        public string Catalogo(string para, string ruta)
        {
            MailMessage mmsg = new MailMessage();
            mmsg.To.Add(para);
            mmsg.Subject = "Solicitud de catalogo";
            mmsg.SubjectEncoding = Encoding.UTF8;

            string mensaje ="Buen dia,"+"\n"+"\n"+ "Le hacemos presente el catalogo de productos que usted solicito"+ "\n"+"\n"+"Saludos.";

            mmsg.Attachments.Add(new Attachment(@ruta));
            mmsg.Body = mensaje;
            mmsg.BodyEncoding = Encoding.UTF8;
            mmsg.From = new MailAddress("tiendagg.mensajero@gmail.com");

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("tiendagg.mensajero@gmail.com", "Tgg246810");

            string output;

            try
            {
                smtp.Send(mmsg);
                mmsg.Dispose();
                output = "El catalogo fue enviado exitosamente";
            }
            catch (Exception ex) { output = "Error al enviar correo : " + ex.Message; }

            return output;
        }

    }
}