using System.Runtime.Serialization;
using System.Text;

namespace NetEti.ObjectSerializer
{
    /// <summary>
    /// Bietet Unterstützung für die Serialisierung und De-Serialisierung von Objekten.
    /// </summary>
    /// <remarks>
    /// Autor: Erik Nagel
    ///
    /// 15.03.2015 Erik Nagel: erstellt
    /// </remarks>  public class XMLSerializationUtility
    public class SerializationUtility
    {
        #region public members

        /*
        /// <summary>
        /// Deserialisiert einen String mit einem gegebenen Encoding.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp, z.B. Base64.</param>
        /// <param name="encoded">Der verschlüsselte String.</param>
        /// <returns>Das deserialisierte Objekt.</returns>
        public static object DeserializeObject(Encoding encoding, string encoded)
        {
            using (MemoryStream memoryStream = new MemoryStream(StringToByteArray(encoding, encoded)))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(memoryStream);
            }
        }
        */

        /*
        /// <summary>
        /// Deserialisiert einen String mit einem gegebenen Encoding.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp, z.B. Base64.</param>
        /// <param name="encoded">Der verschlüsselte String.</param>
        /// <returns>Das deserialisierte Objekt.</returns>
        public static object DeserializeObject(Encoding encoding, string encoded)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(object));
            using (MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(encoded)))
            {
                return serializer.ReadObject(memoryStream);
            }
        }
        */

        /*
        /// <summary>
        /// Serialisiert ein Objekt in einen Base64-kodierten String.
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt.</param>
        /// <returns>String mit Base64-Repräsentation des Objekts.</returns>
        public static string SerializeObjectToBase64(object obj)
        {
            byte[] serialized = XMLSerializationUtility.serializeObjectToByteArray(Encoding.Default, obj);
            if (serialized != null)
            {
                return Convert.ToBase64String(serialized);
            }
            else
            {
                return string.Empty;
            }
        }
        */

        /*
        /// <summary>
        /// Deserialisiert einen Base64-String in ein Objekt.
        /// </summary>
        /// <param name="encoded">String mit Base64-Repräsentation des Objekts.</param>
        /// <returns>Das ursprünglich serialisierte Objekt.</returns>
        public static object DeserializeObjectFromBase64(string encoded)
        {
            byte[] serialized = Convert.FromBase64String(encoded);
            return XMLSerializationUtility.DeserializeObject(Encoding.Default, ByteArrayToString(Encoding.Default, serialized));
        }
        */

        // Chappy (ChatGPT):
        /// <summary>
        /// Serialisiert ein Objekt in einen Base64-kodierten String.
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt.</param>
        /// <returns>String mit Base64-Repräsentation des Objekts.</returns>
        public static string SerializeObjectToBase64(object obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                byte[] serializedBytes = memoryStream.ToArray();
                return Convert.ToBase64String(serializedBytes);
            }
        }

        /// <summary>
        /// Deserialisiert einen Base64-String in ein Objekt.
        /// </summary>
        /// <param name="serializedObj">String mit Base64-Repräsentation des Objekts.</param>
        /// <returns>Das ursprünglich serialisierte Objekt.</returns>
        public static T? DeserializeObjectFromBase64<T>(string serializedObj)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            byte[] serializedBytes = Convert.FromBase64String(serializedObj);
            using (MemoryStream memoryStream = new MemoryStream(serializedBytes))
            {
                return (T?)serializer.ReadObject(memoryStream);
            }
        }

        /// <summary>
        /// Serialisiert ein Objekt in einen UTF8-kodierten String.
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt.</param>
        /// <returns>String mit Base64-Repräsentation des Objekts.</returns>
        public static string SerializeObjectToUTF8(object obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Deserialisiert einen UTF8-String in ein Objekt.
        /// </summary>
        /// <param name="serializedObj">String mit UTF8-Repräsentation des Objekts.</param>
        /// <returns>Das ursprünglich serialisierte Objekt.</returns>
        public static T? DeserializeObjectFromUTF8<T>(string serializedObj)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedObj)))
            {
                return (T?)serializer.ReadObject(memoryStream);
            }
        }

        /*
        /// <summary>
        /// Serialisiert ein Objekt in einen String mit einem gegebenen Encoding.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp, unterstützt werden Base64 und UTF8.</param>
        /// <param name="obj">Das zu serialisierende Objekt.</param>
        /// <returns>Das Objekt als verschlüsselter String.</returns>
        public static string SerializeObject(Encoding encoding, object obj)
        {
            byte[] serialized = XMLSerializationUtility.serializeObjectToByteArray(encoding, obj);
            if (serialized != null)
            {
                return ByteArrayToString(encoding, serialized);
            }
            else
            {
                return string.Empty;
            }
        }
        */

        /// <summary>
        /// Standard-Konstruktor.
        /// </summary>
        public SerializationUtility() { }
        #endregion public members

        #region private members

        /*
        /// <summary>
        /// Serialisiert ein Objekt in ein Byte-Array mit einem gegebenen Encoding.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp, z.B. Base64.</param>
        /// <param name="obj">Das zu serialisierende Objekt.</param>
        /// <returns>Das Byte-Array in der entsprechenden Verschlüsselung.</returns>
        private static byte[] serializeObjectToByteArray(Encoding encoding, object obj)
        {
            if (obj == null)
            {
                return null;
            }
            /* 22.02.2023 Nagel: Fehler	SYSLIB0011	"BinaryFormatter.Serialize(Stream, object)" ist veraltet:
             * "BinaryFormatter serialization is obsolete and should not be used.
             * See https://aka.ms/binaryformatter for more information."
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);
            

            return memoryStream.ToArray();
        }
        */

        /// <summary>
        /// Deserialisiert einen verschlüsselten String in ein Byte-Array.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp, z.B. Base64.</param>
        /// <param name="encoded">In einen String Verschlüsseltes Objekt.</param>
        /// <returns>Der entschlüsselte String als Byte-Array.</returns>
        private static Byte[] StringToByteArray(Encoding encoding, string encoded)
        {
            return encoding.GetBytes(encoded);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp, z.B. Base64.</param>
        /// <param name="byteArray">Das zu verschlüsselnde Byte-Array.</param>
        /// <returns>String mit dem verschlüsselten Byte-Array.</returns>
        private static string ByteArrayToString(Encoding encoding, byte[] byteArray)
        {
            return encoding.GetString(byteArray);
        }

        #endregion private members
    }
}