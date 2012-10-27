using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Xml;

namespace ROZSED.Std
{
    public static class XML
    {
        /// <summary>
        /// Save object to XML file using XMLSerializer class.
        /// </summary>
        public static void ToXML<T>(this T obj, string path)
        {
            XmlSerializer writer = new XmlSerializer(typeof(T));
            using (StreamWriter file = new StreamWriter(path))
                writer.Serialize(file, obj);
        }
        /// <summary>
        /// Save object to XML file using XMLSerializer class.
        /// </summary>
        public static void ToXML<T>(this T obj, TextWriter sw)
        {
            XmlSerializer writer = new XmlSerializer(typeof(T));
            writer.Serialize(sw, obj);
        }
        /// <summary>
        /// Read XML file to object of type <paramref name="T"/> using XmlSerializer.
        /// </summary>
        /// <param name="path">Path to XML file.</param>
        public static T Read<T>(string path)
        {
            XmlSerializer reader = new XmlSerializer(typeof(T));
            using (XmlReader file = XmlReader.Create(path)) // read the XML file.
                return (T)reader.Deserialize(file); // deserialize the content of the file into a object.
        }
    }
}
