using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Proyecto_DSWI_TiendaGG.Models;
using System.Configuration;

namespace Proyecto_DSWI_TiendaGG.Modulo
{
    public class ProductoDAO
    {
        public IEnumerable<Producto> productos()
        {
            List<Producto> temporal = new List<Producto>();

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from tb_producto", cn);
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Producto p = new Producto()
                    {
                        cod_pro = dr.GetString(0),
                        desc_pro = dr.GetString(1),
                        pre_pro = dr.GetDecimal(2),
                        stock_pro = dr.GetInt32(3),
                        cod_cat = dr.GetString(4),
                        est_pro = dr.GetInt32(5),
                        img_pro = dr.GetString(6)
                    };
                    temporal.Add(p);
                }
                dr.Close(); cn.Close();
            }

            return temporal;
        }

        public IEnumerable<Producto> consultaMultiple(List<SqlParameter> ps)
        {
            List<Producto> temporal = new List<Producto>();

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("usp_consulta_multiple", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(ps.ToArray());
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Producto p = new Producto()
                    {
                        cod_pro = dr.GetString(0),
                        desc_pro = dr.GetString(1),
                        pre_pro = dr.GetDecimal(2),
                        stock_pro = dr.GetInt32(3),
                        cod_cat = dr.GetString(4),
                        est_pro = dr.GetInt32(5),
                        img_pro = dr.GetString(6)
                    };
                    temporal.Add(p);
                }
                dr.Close(); cn.Close();
            }

            return temporal;
        }

        public IEnumerable<Categoria> categorias()
        {
            List<Categoria> temporal = new List<Categoria>();

            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("select * from tb_Categoria", cn);
                cn.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                Categoria c = new Categoria();
                    c.cod_cat = "C00";
                    c.nom_cat = "Todos";
                    temporal.Add(c);


                while (dr.Read())
                {
                    c = new Categoria()
                    {
                        cod_cat = dr.GetString(0),
                        nom_cat = dr.GetString(1)
                    };
                    temporal.Add(c);
                }
                dr.Close(); cn.Close();
            }

            return temporal;
        }

        public Producto Buscar(string cod)
        {
            return productos().Where(p => p.cod_pro == cod).FirstOrDefault();
        }

        public string CRUD(string sp, List<SqlParameter> parametros, int op)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sp, cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parametros.ToArray());
                    cn.Open();

                    int n = cmd.ExecuteNonQuery();
                    if (op == 1) mensaje = n + "Producto agregado";
                    else if (op == 2) mensaje = n + "Producto actualizado";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }

        public string Descontinuar(string cod)
        {
            string mensaje = "";
            using (SqlConnection cn = new SqlConnection(
                ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
            {
                try
                {
                    Producto p = Buscar(cod);
                    SqlCommand cmd = new SqlCommand("usp_delete_producto", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cod", cod);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    mensaje = "Producto "+ p.desc_pro +" descontinuado";
                }
                catch (SqlException ex) { mensaje = ex.Message; }
                finally { cn.Close(); }
            }
            return mensaje;
        }
    }
}