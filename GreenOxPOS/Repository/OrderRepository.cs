using GreenOxPOS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GreenOxPOS.Repository
{
    public class OrderRepository
    {
        private static SqlConnection conn;

        private static void connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["GreenOxConn"].ToString();
            conn = new SqlConnection(conStr);
        }
        public bool PlaceOrder(Order o, string RequestType)
        {
            connection();
            try
            {
                SqlCommand cmd = new SqlCommand("su_sp_Order", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestType", RequestType);
                cmd.Parameters.AddWithValue("@Fk_Customer_Address_Id", o.Address.Id);
                cmd.Parameters.AddWithValue("@Fk_Product_Id", o.Product);
                cmd.Parameters.AddWithValue("@Fk_Product_Quantity", o.Quantity);
                cmd.Parameters.AddWithValue("@Fk_PerProduct_Amount", o.ProductAmount);
                cmd.Parameters.AddWithValue("@Discount_OnAmount", o.DiscountAmount);
                cmd.Parameters.AddWithValue("@Payment", o.Payment);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                return true;

            }
            catch (Exception ex)
            {
                Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        public bool AddEditCustomer(Address a, string RequestType)
        {
            connection();
            try
            {
                SqlCommand cmd = new SqlCommand("su_sp_CustomerCreate", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestType", RequestType);
                cmd.Parameters.AddWithValue("@Name", a.Customer.Name);
                cmd.Parameters.AddWithValue("@email", a.Customer.Email);
                cmd.Parameters.AddWithValue("@phoneNo", a.Customer.PhoneNo);
                cmd.Parameters.AddWithValue("@Address", a.CustAddress);
                cmd.Parameters.AddWithValue("@AddrId", a.Id);

                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt));
                cmd.Parameters["@id"].Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                a.Id = Formatting.ConvertNullToInt64(cmd.Parameters["@id"].Value);

                return true;

            }
            catch (Exception ex)
            {
                Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        public bool SearchCustomer(string SearchTitle, string SearchText, ref List<Address> ad)
        {

            connection();
            try
            {
                SqlCommand cmd = new SqlCommand("su_sp_Customer", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestType", SearchTitle);
                cmd.Parameters.AddWithValue("@SearchText", SearchText);

                conn.Open();
                 
                SqlDataReader dr = cmd.ExecuteReader();
                Address a;
                Customer c;
                while (dr.Read())
                {
                    a = new Address();
                    c = new Customer();
                    a.Id = Formatting.ConvertNullToInt64(dr["id"]);
                    a.CustAddress = Formatting.ConvertNullToString(dr["Address"]);
                    c.Name = Formatting.ConvertNullToString(dr["Name"]);
                    c.Email = Formatting.ConvertNullToString(dr["email"]);
                    c.PhoneNo = Formatting.ConvertNullToString(dr["phoneNo"]);
                    a.Customer = c;

                    ad.Add(a);

                }
                dr.Close();
                conn.Close();

                if (ad.Count() > 0)
                    return true;
                else return false;

            }
            catch (Exception ex)
            {
                Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

                return false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

        }

        public static void Errorlog(Exception ex, string FormName, string FunctionName)
        {
            connection();
            try
            {
                string Query = "insert into sais_admin..ErrorLog (Message, Stack, FormName, FunctionName) ";
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