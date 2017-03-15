using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenOxPOS.Repository
{
    public class Formatting
    {
        public static DateTime ConvertNullToDateTime(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    return Convert.ToDateTime(oValue);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            catch { return DateTime.MinValue; }
        }

        public static string ConvertNullToString(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    return oValue.ToString().Trim();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static int ConvertNullToToInt16(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    short i = 0;
                    Boolean isShort = Int16.TryParse(oValue.ToString(), out i);
                    if (isShort == true)
                    {
                        return Convert.ToInt16(oValue);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static int ConvertNullToInt32(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    int i = 0;
                    Boolean isInt = Int32.TryParse(oValue.ToString(), out i);
                    if (isInt == true)
                    {
                        return Convert.ToInt32(oValue);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static Int64 ConvertNullToInt64(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    Int64 i = 0;
                    Boolean isInt = Int64.TryParse(oValue.ToString(), out i);
                    if (isInt == true)
                    {
                        return Convert.ToInt64(oValue);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }

        public static decimal ConvertNullToDecimal(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    decimal i = 0;
                    Boolean isDecimal = Decimal.TryParse(oValue.ToString(), out i);
                    if (isDecimal == true)
                    {
                        return Convert.ToDecimal(oValue);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        public static float ConvertNullToFloat(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    float i = 0;
                    Boolean isFloat = Single.TryParse(oValue.ToString(), out i);
                    if (isFloat == true)
                    {
                        return i;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string ConvertDateTimeToAmPmString(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    DateTime dt;
                    bool IsDate = DateTime.TryParse(oValue.ToString(), out dt);
                    if (IsDate == true)
                    {
                        return string.Format("{0:MM/dd/yyyy h:mm:ss tt}", dt);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public static double ConvertNullToDouble(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    decimal i = 0;
                    Boolean isDecimal = Decimal.TryParse(oValue.ToString(), out i);
                    if (isDecimal == true)
                    {
                        return Convert.ToDouble(oValue);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        public static bool ConvertNullToBoolean(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    return Convert.ToBoolean(oValue);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string ConvertDateTimeToShortString(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    DateTime dt;
                    bool IsDate = DateTime.TryParse(oValue.ToString(), out dt);
                    if (IsDate == true)
                    {
                        return string.Format("{0:MM/dd/yyyy}", dt);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        //suyash --convert to date time with am/pm
        public static string ConvertDateTimeToDateTimeString(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    DateTime dt;
                    bool IsDate = DateTime.TryParse(oValue.ToString(), out dt);
                    if (IsDate == true)
                    {
                        return string.Format("{0:MM/dd/yyyy h:mm:ss tt}", dt);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string ConvertDateTimeToDateTime24FmtString(object oValue)
        {
            try
            {
                if (oValue != System.DBNull.Value)
                {
                    DateTime dt;
                    bool IsDate = DateTime.TryParse(oValue.ToString(), out dt);
                    if (IsDate == true)
                    {
                        return string.Format("{0:MM/dd/yyyy HH:mm:ss}", dt);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}