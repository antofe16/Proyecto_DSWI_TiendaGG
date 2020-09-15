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
                SqlCommand cmd = new SqlCommand("select * from tb_Producto",cn);
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
    }
}