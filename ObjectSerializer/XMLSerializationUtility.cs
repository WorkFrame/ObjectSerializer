using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NetEti.ObjectSerializer
{
  /// <summary>
  /// Bietet Unterstützung für die Serialisierung und De-Serialisierung von Objekten.
  /// </summary>
  /// <remarks>
  /// File: XMLSerializationUtility.cs
  /// Autor: Erik Nagel
  ///
  /// 15.03.2015 Erik Nagel: erstellt
  /// </remarks>  public class XMLSerializationUtility
  public class XMLSerializationUtility
  {
    #region public members

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

    /// <summary>
    /// Serialisiert ein Objekt in einen String mit einem gegebenen Encoding.
    /// </summary>
    /// <param name="encoding">Verschlüsselungstyp, z.B. Base64.</param>
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

    /// <summary>
    /// Standard-Konstruktor.
    /// </summary>
    public XMLSerializationUtility() { }
    #endregion public members

    #region private members

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
      MemoryStream memoryStream = new MemoryStream();
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      binaryFormatter.Serialize(memoryStream, obj);
      return memoryStream.ToArray();
    }

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