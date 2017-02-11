using System;
using System.Data.SqlClient;

using System.Configuration;
using GreenOxPOS.Models;
using System.Data;
using System.Collections.Generic;
namespace GreenOxPOS.Repository
{
    public class ProductRepository
    {
        private static SqlConnection conn;

        private static void connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["GreenOxConn"].ToString();
            conn = new SqlConnection(conStr);
        }

        public bool Product(Product p, string RequestType)
        {
            connection();
            try
            {
                SqlCommand cmd = new SqlCommand("su_sp_Product", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestType", RequestType);
                cmd.Parameters.AddWithValue("@Id", p.ProductId);
                cmd.Parameters.AddWithValue("@ProdName", p.ProductName);
                cmd.Parameters.AddWithValue("@ProductCategory", p.ProductCategory.PCId);
                cmd.Parameters.AddWithValue("@Price", p.Price);
                if (RequestType == "ADDPRODUCTCAT" || RequestType == "ADDPRODUCT")
                {
                    cmd.Parameters.Add(new SqlParameter("@OutputId", SqlDbType.Int));
                    cmd.Parameters["@OutputId"].Direction = ParameterDirection.Output;

                }
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                if (RequestType == "ADDPRODUCTCAT" || RequestType == "ADDPRODUCT")
                {
                    p.ProductId=Formatting.ConvertNullToInt32(cmd.Parameters["@OutputId"].Value);
                }
                return true;



            }
            catch (Exception ex)
            {
                Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                p.ProductName = ex.Message;
                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
        public List<Product> GetAllProduct(string RequestType, int id = 0)
        {
            connection();
            List<Product> pro = new List<Product>();
            try
            {
                SqlCommand cmd = new SqlCommand("su_sp_Product", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestType", RequestType);
                cmd.Parameters.AddWithValue("@id", id);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                conn.Open();
                sda.Fill(dt);
                conn.Close();
                List<ProductCategory> pc = new List<ProductCategory>();
                int pcId = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    pcId = Formatting.ConvertNullToInt32(dr["Fk_ProductCategory"]);
                    ProductCategory p = null;
                    foreach (ProductCategory pcItem in pc)
                    {
                        if (pcItem.PCId == pcId)
                        {
                            p = pcItem;
                            break;
                        }
                    }
                    if (p == null)
                    {
                        p = new ProductCategory()
                        {
                            PCId = pcId,
                            Name = Formatting.ConvertNullToString(dr["Name"])
                        };
                        pc.Add(p);
                    }
                    pro.Add(
                        new Product
                        {
                            ProductId = Formatting.ConvertNullToInt32(dr["id"]),
                            ProductName = Formatting.ConvertNullToString(dr["pName"]),
                            Price = Formatting.ConvertNullToDecimal(dr["Price"]),
                            IsActive = Formatting.ConvertNullToBoolean(dr["IsActive"]),
                            ProductCategory = p

                        });
                }


            }
            catch (Exception ex)
            {
                Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
            return pro;
        }

        public List<ProductCategory> GetAllProductCategory(string RequestType)
        {
            connection();
            List<ProductCategory> pc = new List<ProductCategory>();
            try
            {
                SqlCommand cmd = new SqlCommand("su_sp_Product", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestType", RequestType);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                conn.Open();
                sda.Fill(dt);
                conn.Close();


                foreach (DataRow dr in dt.Rows)
                {
                    pc.Add(
                        new ProductCategory
                        {
                            PCId = Formatting.ConvertNullToInt32(dr["id"]),
                            Name = Formatting.ConvertNullToString(dr["Name"]),
                            IsActive = Formatting.ConvertNullToBoolean(dr["IsActive"])
                        });
                }

            }
            catch (Exception ex)
            {
                Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
            return pc;
        }

        public static void Errorlog(Exception ex, string FormName, string FunctionName)
        {
            connection();
            try
            {
                string Query = "insert into ErrorLog (Message, Stack, FormName, FunctionName) ";
                Query += "values(@Message, @Stack, @FormName, @FunctionName)";


                SqlCommand cmd = new SqlCommand(Query, conn);
                cmd.Parameters.AddWithValue("@Message", (ex.InnerException != null) ? ex.InnerException.ToString().Replace("'", "\"") : ex.Message);
                cmd.Parameters.AddWithValue("@Stack", ex.StackTrace.ToString().Replace("'", "\""));
                cmd.Parameters.AddWithValue("@FormName", FormName);
                cmd.Parameters.AddWithValue("@FunctionName", FunctionName.Replace(".ctor", "Constructor"));

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();



            }
            catch
            {

            }
        }

    }
}