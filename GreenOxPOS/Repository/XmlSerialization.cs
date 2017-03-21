using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Xml.Serialization;

namespace GreenOxPOS.Repository
{
    public class XmlSerialization
    {
        public static Object Deserialize(object objDeserialize, string Path)
        {
            FileStream fs = null;
            try
            {
                if (File.Exists(Path))
                {
                    XmlSerializer xDe = new XmlSerializer(objDeserialize.GetType());
                    fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    Object obj = (Object)xDe.Deserialize(fs);
                    return obj;
                }
                else
                {
                    throw new Exception("File not found.");
                }
            }
            catch { throw; }
            finally
            {
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }

        public static void SerializeXml(Object objSerialize, string sPath)
        {
            FileStream fs = null;
            try
            {
                XmlSerializer xSer = new XmlSerializer(objSerialize.GetType());
                fs = new FileStream(sPath, FileMode.Create, FileAccess.Write, FileShare.Write);
                xSer.Serialize(fs, objSerialize);
            }
            catch { throw; }
            finally
            {
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }

        public static string SerializeToString(object obj)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    return writer.ToString();
                }
            }
            catch { throw; }
        }

        public static void SerializeObject(object obj, string Path)
        {
            Stream stream = null;
            try
            {
                stream = File.Open(Path, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, obj);

            }
            catch { throw; }
            finally
            {
                stream.Flush();
                stream.Close();
                stream.Dispose();
            }
        }

        public static object DeserializeObject(string Path)
        {
            FileStream fs = null;
            try
            {
                if (File.Exists(Path))
                {
                    fs = new FileStream(Path, FileMode.Open);
                    BinaryFormatter bformatter = new BinaryFormatter();
                    fs.Position = 0;
                    return bformatter.Deserialize(fs);
                }
                else
                {
                    throw new Exception("File not found.");
                }
            }
            catch { throw; }
            finally
            {
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }

        public static object DeserializeFromString(object objDeserialize, string Data)
        {
            try
            {
                object obj;
                XmlSerializer xDe = new XmlSerializer(objDeserialize.GetType());
                using (TextReader reader = new StringReader(Data))
                {
                    obj = xDe.Deserialize(reader);
                }
                return obj;
            }
            catch { throw; }
        }
    }
}