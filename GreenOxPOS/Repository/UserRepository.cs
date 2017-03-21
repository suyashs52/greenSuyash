using System;
using System.Data.SqlClient;

using System.Configuration;
using GreenOxPOS.Models;
using System.Data;
using System.Collections.Generic;

namespace GreenOxPOS.Repository
{
    public class UserRepository : ErrorRepository
    {
        private static SqlConnection conn;

        private static void connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["GreenOxConn"].ToString();
            conn = new SqlConnection(conStr);
        }
        /// <summary>
        /// bool:admin,bool2:validate
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public Tuple<bool, bool> Credential(User u)
        {
            connection();
            try
            {
                SqlCommand cmd = new SqlCommand("su_sp_ValidateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", u.UserName);
                cmd.Parameters.AddWithValue("@password", u.Password);
                cmd.Parameters.Add(new SqlParameter("@isAdmin", SqlDbType.Bit));
                cmd.Parameters["@isAdmin"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@isValidated", SqlDbType.Bit));
                cmd.Parameters["@isValidated"].Direction = ParameterDirection.Output;



                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                return new Tuple<bool, bool>(Formatting.ConvertNullToBoolean(cmd.Parameters["@isAdmin"].Value),
                    Formatting.ConvertNullToBoolean(cmd.Parameters["@isValidated"].Value));

            }
            catch (Exception ex)
            {
                Errorlog(ex, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return new Tuple<bool, bool>(false, false);

            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

    }
}