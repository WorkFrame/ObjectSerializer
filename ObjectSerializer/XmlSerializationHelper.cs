using System.Xml.Serialization;

namespace NetEti.ObjectSerializer
{
    /// <summary>
    /// Statische Klasse mit Methoden zum Serialisieren und Deserialisieren von Objekten nach und aus Xml.
    /// </summary>
    public static class XmlSerializationHelper
    {
        /// <summary>
        /// Serialisiert einen JobContainer in ein XML-Format.
        /// </summary>
        /// <param name="obj">Zu serialisierendes Objekt.</param>
        /// <returns>String mit dem serialisierten Objekt.</returns>
        /// <exception cref="Exception">Mögliche Fehler bei der Serialisierung des Objekts.</exception>
        public static string SerializeToXml<T>(T obj)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Serialisieren von {typeof(T)} zu XML", ex);
            }
        }

        /// <summary>
        /// Deserialisiert ein Objekt aus einem XML-Format.
        /// </summary>
        /// <param name="xml">Xml-String mit einem serialisierten Objekt.</param>
        /// <returns>Aus einem Xml-String deserialisiertes Objekt oder null.</returns>
        /// <exception cref="Exception">Mögliche Fehler bei der Deserialisierung des Objekts.</exception>
        public static T? DeserializeFromXml<T>(string xml)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringReader reader = new StringReader(xml))
                {
                    return (T?)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Deserialisieren von XML zu {typeof(T)}", ex);
            }
        }

        /// <summary>
        /// Lädt ein Objekt aus einer XML-Datei.
        /// </summary>
        /// <param name="filePath">Pfad zur Xml-Datei.</param>
        /// <returns>Aus einer Xml-Datei geladenes Objekt.</returns>
        /// <exception cref="Exception">Mögliche Fehler beim Laden des Objekts aus einer Xml-Datei.</exception>
        public static T? LoadFromXmlFile<T>(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    return (T?)serializer.Deserialize(fileStream);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading Object from XML file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Speichert ein Objekt in eine Xml-Datei.
        /// </summary>
        /// <param name="obj">Zu speicherndes Objekt.</param>
        /// <param name="filePath">Pfad zur Xml-Datei.</param>
        /// <exception cref="Exception">Mögliche Fehler beim Speichern des Objekts.</exception>
        public static void SaveToXmlFile<T>(T obj, string filePath)
        {
            try
            {
                string xmlString = SerializeToXml<T>(obj);
                File.WriteAllText(filePath, xmlString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving Object to Xml file: {filePath}", ex);
            }
        }

    }
}
