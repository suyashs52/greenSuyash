using GreenOxPOS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace GreenOxPOS.Repository
{
    public class OrderRepository : ErrorRepository
    {
        private static SqlConnection conn;

        private static void connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["GreenOxConn"].ToString();
            conn = new SqlConnection(conStr);
        }
        public bool PlaceOrder(Order o, string UserName, string RequestType)
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
                cmd.Parameters.AddWithValue("@Fk_UserName", UserName);
                cmd.Parameters.Add(new SqlParameter("@OrderId", SqlDbType.BigInt));
                cmd.Parameters["@OrderId"].Direction = ParameterDirection.Output;

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                o.id = Formatting.ConvertNullToInt64(cmd.Parameters["@OrderId"].Value);


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
        public long ListOrder(DateTime From, DateTime To, int size, int offset, ref List<Order> o, string RequestType = "ListOrder")
        {
            connection();
            try
            {
                SqlCommand cmd = new SqlCommand("su_sp_ListOrder", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RequestType", RequestType);
                cmd.Parameters.AddWithValue("@StartDate", From);
                cmd.Parameters.AddWithValue("@EndDate", To);
                cmd.Parameters.AddWithValue("@PageNo", offset);
                cmd.Parameters.AddWithValue("@PageSize", size);
                cmd.Parameters.Add(new SqlParameter("@TotalCount", SqlDbType.BigInt));
                cmd.Parameters["@TotalCount"].Direction = ParameterDirection.Output;


                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                Order or;

                while (dr.Read())
                {
                    or = new Order();
                    or.id = Formatting.ConvertNullToInt64(dr["id"]);
                    or.Product = Formatting.ConvertNullToString(dr["Fk_Product_Id"]);
                    or.Quantity = Formatting.ConvertNullToString(dr["Fk_Product_Quantity"]);
                    or.ProductAmount = Formatting.ConvertNullToString(dr["Fk_PerProduct_Amount"]);
                    or.DiscountAmount = Formatting.ConvertNullToFloat(dr["Discount_OnAmount"]);
                    or.Payment = Formatting.ConvertNullToFloat(dr["Payment"]);
                    or.CreatedOn = Formatting.ConvertNullToDateTime(dr["CreatedOn"]);
                    or.CreatedBy = Formatting.ConvertNullToString(dr["username"]);
                    or.Address = new Address();
                    or.Address.Id = Formatting.ConvertNullToInt64(dr["Fk_Cusomer_Address_Id"]);
                    or.Address.CustAddress = Formatting.ConvertNullToString(dr["Name"]) + "," + Formatting.ConvertNullToString(dr["PhoneNo"]);


                    o.Add(or);

                }


                dr.Close();
                conn.Close();

                return Formatting.ConvertNullToInt64(cmd.Parameters["@TotalCount"].Value);

            }
            catch (Exception ex)
            {
                Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

                return 0;
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

      

    }



}