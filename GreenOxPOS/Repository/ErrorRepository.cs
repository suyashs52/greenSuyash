using System;

using System.Configuration;
using System.Data.SqlClient;
using System.IO;


namespace GreenOxPOS.Repository
{
    public class ErrorRepository
    {
        private static SqlConnection conn;

        private static void connection()
        {
            string conStr = ConfigurationManager.ConnectionStrings["GreenOxConn"].ToString();
            conn = new SqlConnection(conStr);
        }
        public static void Errorlog(Exception ex, string FormName, string FunctionName)
        {
            connection();
            try
            {
                string Query = "insert into ssp..ErrorLog (Message, Stack, FormName, FunctionName,CreatedOn) ";
                Query += "values(@Message, @Stack, @FormName, @FunctionName,getdate())";


                SqlCommand cmd = new SqlCommand(Query, conn);
                cmd.Parameters.AddWithValue("@Message", (ex.InnerException != null) ? ex.InnerException.ToString().Replace("'", "\"") : ex.Message);
                cmd.Parameters.AddWithValue("@Stack", ex.StackTrace.ToString().Replace("'", "\""));
                cmd.Parameters.AddWithValue("@FormName", FormName);
                cmd.Parameters.AddWithValue("@FunctionName", FunctionName.Replace(".ctor", "Constructor"));

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();



            }
            catch (Exception exc)
            {
                SerializeError(exc, FormName, FunctionName);
            }
        }
        static void SerializeError(Exception ex, string FormName, string FunctionName)
        {
            try
            {
                string LogPath = ConfigurationManager.AppSettings["logpath"];
                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                string Path = LogPath + ConfigurationManager.AppSettings["filename"]; //"ErrorLog.xml";
                ExceptionLog el = new ExceptionLog();
                if (File.Exists(Path))
                {
                    el = XmlSerialization.Deserialize(el, Path) as ExceptionLog;
                }

                ExceptionFields ef = new ExceptionFields();
                ef.Name = ex.ToString();
                ef.Message = ex.Message;
                ef.Stack = ex.StackTrace.ToString();
                ef.Source = ex.Source.ToString();
                ef.Target = ex.TargetSite.ToString();
                ef.Form = FormName;
                ef.Function = FunctionName;
                ef.OccuredOn = DateTime.Now.ToString();
                el.ExceptionDetail.Add(ef);
                XmlSerialization.SerializeXml(el, Path);
            }
            catch (Exception exc) { throw exc; }

        }

    }

}
