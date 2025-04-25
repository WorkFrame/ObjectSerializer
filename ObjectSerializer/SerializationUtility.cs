using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Serialization;

namespace NetEti.ObjectSerializer
{
    /// <summary>
    /// AnonymousDataMember oder AnonymousDataMemberAttribute kennzeichnet
    /// einen DataMember, der als 'object' deklariert wird, der aber zur Laufzeit
    /// Instanzen verschiedener unbekannter Typen enthalten kann.
    /// Hier kann nicht direkt über [DataContract] und [DataMember] serialisiert
    /// werden, sondern es muss eine gesonderte Verarbeitung erfolgen.
    /// </summary>
    public class AnonymousDataMemberAttribute : Attribute
    {
    }

    /// <summary>
    /// Bietet Unterstützung für die Serialisierung und De-Serialisierung von Objekten.
    /// </summary>
    /// <remarks>
    /// Autor: Erik Nagel
    ///
    /// 15.03.2015 Erik Nagel: erstellt.
    /// 18.06.2023 Erik Nagel: im Zuge der Portierung auf .Net7 musste  dies Library komplett
    ///            überarbeitet werden:
    ///            SYSLIB0011 "BinaryFormatter.Serialize(Stream, object)" ist veraltet:
    ///            "BinaryFormatter serialization is obsolete and should not be used.
    ///            See https://aka.ms/binaryformatter for more information."
    ///            Thanks to Brian Sullivan and Dzyann for their help on de-serializing Types:
    ///            https://stackoverflow.com/questions/12306/can-i-serialize-a-c-sharp-type-object
    /// 31.08.2023 Erik Nagel: Neuen Schalter anonymousToString implementiert.
    /// 25.04.2025 Erik Nagel: Neue Routinen für Json und Xml hinzugefügt; Klasse statisch gemacht.
    /// </remarks>
    public static class SerializationUtility
    {
        #region Base64

        /// <summary>
        /// Serialisiert ein Objekt in einen String mit einem gegebenen Encoding.
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt.</param>
        /// <param name="anonymousToString">Bei True werden enthaltene anonyme Datenobjekte
        /// nur als Strings verschlüsselt; Default: false.</param>
        /// <returns>Das Objekt als verschlüsselter String.</returns>
        public static string SerializeObjectToBase64String(object obj, bool anonymousToString = false)
        {
            string? result = null;
            byte[] serialized = SerializeObjectToByteList(obj, anonymousToString).ToArray();
            if (serialized != null)
            {
                result = ByteArrayToBase64String(serialized);
            }
            if (result == null)
            {
                throw new ArgumentException("Das Objekt konnte nicht serialisiert werden.");
            }
            return result;
        }

        /// <summary>
        /// Deserialisiert eine Byte-Liste in ein entsprechendes Objekt.
        /// </summary>
        /// <param name="encoded">Codierter string mit der Repräsentation des Objekts.</param>
        /// <returns>Das deserialisierte Objekt.</returns>
        public static object? DeserializeObjectFromBase64String(string encoded)
        {
            byte[] parsed = new byte[0];
            byte[] buffer = Base64StringToByteArray(encoded);
            if (buffer.Length > 0)
            {
                return DeserializeObjectFromByteList(buffer.ToList());
            }
            else
            {
                throw new ArgumentException("encoded konnte nicht dekodiert werden.");
            }
        }

        #endregion Base64

        #region Encoded String

        /// <summary>
        /// Serialisiert ein Objekt in einen String mit einem gegebenen Encoding.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp (System.Text.Encoding).</param>
        /// <param name="obj">Das zu serialisierende Objekt.</param>
        /// <param name="anonymousToString">Bei True werden enthaltene anonyme Datenobjekte
        /// nur als Strings verschlüsselt; Default: false.</param>
        /// <returns>Das Objekt als verschlüsselter String.</returns>
        public static string SerializeObjectToCodedString(Encoding encoding, object obj, bool anonymousToString = false)
        {
            byte[] serialized = SerializeObjectToByteList(obj).ToArray();
            if (serialized != null)
            {
                return ByteArrayToString(encoding, serialized);
            }
            else
            {
                throw new ArgumentException("Das Objekt konnte nicht serialisiert werden.");
            }
        }

        /// <summary>
        /// Deserialisiert eine Byte-Liste in ein entsprechendes Objekt.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp (System.Text.Encoding).</param>
        /// <param name="encoded">Codierter string mit der Repräsentation des Objekts.</param>
        /// <returns>Das deserialisierte Objekt.</returns>
        public static object? DeserializeObjectFromCodedString(Encoding encoding, string encoded)
        {
            byte[] parsed = new byte[0];
            byte[] buffer = StringToByteArray(encoding, encoded);
            if (buffer.Length > 0)
            {
                return DeserializeObjectFromByteList(buffer.ToList());
            }
            else
            {
                throw new ArgumentException("encoded konnte nicht dekodiert werden.");
            }
        }

        #endregion Encoded String

        #region ByteList

        /// <summary>
        /// Serialisiert ein Objekt in eine Byte-Liste.
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt.</param>
        /// <param name="anonymousToString">Bei True werden enthaltene anonyme Datenobjekte
        /// nur als Strings verschlüsselt; Default: false.</param>
        /// <param name="reku">Nur für internen Gebrauch.</param>
        /// <returns>Byte-Liste mit den bytes des Objekts.</returns>
        public static List<byte> SerializeObjectToByteList(object obj, bool anonymousToString = false, int reku = 0)
        {
            List<byte> rtn = new();
            if (anonymousToString && reku++ > 0)
            {
                obj = obj.ToString() ?? String.Empty;
            }
            Type type = obj.GetType();
            TypeInfo typeInfo = type.GetTypeInfo();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(type);
                serializer.WriteObject(memoryStream, obj);
                //-----------------------------------------------------------------------------------------------------
                List<List<byte>> innerObjectByteLists = new();
                var properties = typeInfo.DeclaredProperties;
                AnonymousDataMemberAttribute attr = new AnonymousDataMemberAttribute();
                foreach (var property in properties)
                {
                    var customAttributes = property.GetCustomAttributes();
                    if (customAttributes.Contains(attr))
                    {
                        Console.WriteLine(String.Format("{0}: {1}", property.Name, String.Join(',', customAttributes)));

                        object? innerObject = property.GetValue(obj);
                        if (innerObject != null)
                        {
                            List<byte> innerObjectBytes = SerializeObjectToByteList(innerObject, anonymousToString, reku);
                            innerObjectByteLists.Add(innerObjectBytes);
                            Console.WriteLine(String.Format("{0}", innerObjectBytes));
                        }
                    }
                }
                //-----------------------------------------------------------------------------------------------------

                byte[] serializedBytes = memoryStream.ToArray();
                Int32 typeSize = serializedBytes.Length;
                string typeName = type.FullName ?? throw new ArgumentException("Serializytion: Unbekannter Typ.");
                ItemHeader itemHeader = new ItemHeader(typeSize, typeName);
                byte[] serializedHeader = itemHeader.GetFixedLengthItemHeaderByteArray();
                byte[] allBytes = new byte[serializedHeader.Length + serializedBytes.Length];

                Buffer.BlockCopy(serializedHeader, 0, allBytes, 0, serializedHeader.Length);
                Buffer.BlockCopy(serializedBytes, 0, allBytes, serializedHeader.Length, serializedBytes.Length);

                rtn = new List<byte>(allBytes);
                foreach (List<byte> innerObjectBytes in innerObjectByteLists)
                {
                    if (innerObjectBytes.Count > 0)
                    {
                        rtn.AddRange(innerObjectBytes);
                    }
                }
            }
            return rtn;
        }

        /// <summary>
        /// Deserialisiert eine Byte-Liste in ein entsprechendes Objekt.
        /// </summary>
        /// <param name="serialized">Byte-Liste mit den bytes des Objekts.</param>
        /// <returns>Das deserialisierte Objekt.</returns>
        public static object? DeserializeObjectFromByteList(List<byte> serialized)
        {
            int srcOffset = 0;
            return DeserializeObjectFromByteList(serialized, ref srcOffset);
        }

        /// <summary>
        /// Deserialisiert eine Byte-Liste in ein entsprechendes Objekt.
        /// </summary>
        /// <param name="serialized">Byte-Liste mit den bytes des Objekts.</param>
        /// <param name="srcOffset">im laufe der internen Rekursion wandernder Offset.</param>
        /// <returns>Das deserialisierte Objekt.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static object? DeserializeObjectFromByteList(List<byte> serialized, ref int srcOffset)
        {
            object? returnObject = null;
            byte[] inBytes = serialized.ToArray();

            if (srcOffset < inBytes.Length - ItemHeader.ItemHeaderLength)
            {
                byte[] headerBytes = new byte[ItemHeader.ItemHeaderLength];
                Buffer.BlockCopy(inBytes, srcOffset, headerBytes, 0, headerBytes.Length);
                srcOffset += ItemHeader.ItemHeaderLength;
                ItemHeader itemHeader = new ItemHeader(headerBytes);
                Console.WriteLine(itemHeader.ToString());
                Type realType = GetTypeFromTypeString(itemHeader.TypeName.Trim());
                // Test: object instance = Activator.CreateInstance(realType) ?? throw new ArgumentNullException(nameof(realType));
                TypeInfo typeInfo = realType.GetTypeInfo();
                byte[] itemBytes = new byte[itemHeader.TypeLength];
                Buffer.BlockCopy(inBytes, srcOffset, itemBytes, 0, itemBytes.Length);
                srcOffset += itemBytes.Length;
                using (MemoryStream memoryStream = new MemoryStream(itemBytes))
                {
                    DataContractSerializer serializer = new DataContractSerializer(realType);
                    returnObject = (object?)serializer.ReadObject(memoryStream);
                    var properties = typeInfo.DeclaredProperties;
                    AnonymousDataMemberAttribute attr = new AnonymousDataMemberAttribute();
                    if (properties != null)
                    {
                        foreach (var property in properties)
                        {
                            var customAttributes = property.GetCustomAttributes();
                            if (customAttributes.Contains(attr))
                            {
                                Console.WriteLine(String.Format("{0}: {1}", property.Name, String.Join(',', customAttributes)));

                                object? obj = DeserializeObjectFromByteList(serialized, ref srcOffset);
                                property.SetValue(returnObject, obj);
                            }
                        }
                    }
                }
            }
            return returnObject;
        }

        #endregion ByteList

        #region JsonOrXml

        /// <summary>
        /// Überträgt einen String, der ein Json- oder Xml-Objekt
        /// repräsentiert in das entsprechende Objekt.
        /// </summary>
        /// <param name="jsonOrXml">Ein String, der ein Json- oder Xml-Objekt repräsentiert.</param>
        /// <returns>Eine gefüllte Objekt-Instanz vom Typ T.</returns>
        /// <exception cref="ArgumentException">Wird geworfen, wenn der String 'jsonOrXml' kein gültiges unterstütztes Format hat.</exception>
        public static T? DeserializeFromJsonOrXml<T>(string jsonOrXml)
        {
            T? obj = default(T);
            if (jsonOrXml.Trim().StartsWith('{') || jsonOrXml.Trim().StartsWith('['))
            {
                obj =
                    DeserializeFromJson<T>(jsonOrXml);
            }
            else
            {
                if (jsonOrXml.Trim().StartsWith('<'))
                {
                    obj =
                        DeserializeFromXml<T>(jsonOrXml);
                }
                else
                {
                    throw new ArgumentException(
                        "Der Parameter 'jsonOrXml' hat kein bekanntes Format. Unterstützt werden Xml und Json.");
                }
            }
            return obj;
        }

        #endregion JsonOrXml

        #region Json

        /// <summary>
        /// Serialisiert eine Objekt in ein Json-Format.
        /// </summary>
        /// <param name="obj">Zu serialisierendes Objekt.</param>
        /// <param name="includeFields">Bei true werden auch öffentliche Felder ohne Getter und Setter serielisiert; Default: false.</param>
        /// <returns>String mit dem serialisierten Objekt.</returns>
        /// <exception cref="Exception">Mögliche Fehler bei der Serialisierung des Objekts.</exception>
        public static string SerializeToJson<T>(T obj, bool includeFields = false)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true,
                    IncludeFields = includeFields,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                return JsonSerializer.Serialize<T>(obj, options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Serialisieren von {typeof(T)} zu JSON", ex);
            }
        }

        /// <summary>
        /// Deserialisiert eine Objekt aus einem Json-Format.
        /// </summary>
        /// <param name="json">Json-String mit einem serialisierten Objekt.</param>
        /// <returns>Aus einem Json-String deserialisiertes Objekt oder null.</returns>
        /// <exception cref="Exception">Mögliche Fehler bei der Deserialisierung des Objekts.</exception>
        public static T? DeserializeFromJson<T>(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Deserialisieren von JSON zu {typeof(T)}", ex);
            }
        }

        /// <summary>
        /// Lädt ein Objekt aus einer Json-Datei.
        /// </summary>
        /// <param name="filePath">Pfad zur Json-Datei.</param>
        /// <returns>Aus einer Json-Datei geladenes Objekt.</returns>
        /// <exception cref="Exception">Mögliche Fehler beim Laden des Objekts aus einer Json-Datei.</exception>
        public static T? LoadFromJsonFile<T>(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                return DeserializeFromJson<T>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading Object from JSON file: {filePath}", ex);
            }
        }

        /// <summary>
        /// Speichert ein Objekt in eine Json-Datei.
        /// </summary>
        /// <param name="obj">Zu speicherndes Objekt.</param>
        /// <param name="filePath">Pfad zur Json-Datei.</param>
    	/// <param name="includeFields">Bei true werden auch öffentliche Felder ohne Getter und Setter serielisiert; Default: false.</param>
        /// <exception cref="Exception">Mögliche Fehler beim Speichern des Objekts.</exception>
    	public static void SaveToJsonFile<T>(T obj, string filePath, bool includeFields = false)
        {
            try
            {
                string jsonString = SerializeToJson<T>(obj, includeFields);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving Objekt to JSON file: {filePath}", ex);
            }
        }

        #endregion Json

        #region Xml

        /// <summary>
        /// Serialisiert ein Objekt in ein XML-Format.
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

        #endregion Xml

        #region private helpers

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
        /// <param name="encoded">In einen String Verschlüsseltes Objekt.</param>
        /// <returns>Der entschlüsselte String als Byte-Array.</returns>
        private static Byte[] Base64StringToByteArray(string encoded)
        {
            try
            {
                return Convert.FromBase64String(encoded);
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// Serialisiert ein Byte-Array in einen Base64-verschlüsselten String.
        /// </summary>
        /// <param name="byteArray">Das zu verschlüsselnde Byte-Array.</param>
        /// <returns>Das Byte-Array als Base64 verschlüsselter String.</returns>
        private static string? ByteArrayToBase64String(byte[] byteArray)
        {
            try
            {
                return Convert.ToBase64String(byteArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Deserialisiert einen verschlüsselten String in ein Byte-Array.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp, z.B. Base64.</param>
        /// <param name="encoded">In einen String Verschlüsseltes Objekt.</param>
        /// <returns>Der entschlüsselte String als Byte-Array.</returns>
        private static Byte[] StringToByteArray(Encoding encoding, string encoded)
        {
            try
            {
                return encoding.GetBytes(encoded);
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// Serialisiert ein Byte-Array in einen verschlüsselten String.
        /// </summary>
        /// <param name="encoding">Verschlüsselungstyp, z.B. UTF8.</param>
        /// <param name="byteArray">Das zu verschlüsselnde Byte-Array.</param>
        /// <returns>String mit dem verschlüsselten Byte-Array.</returns>
        private static string ByteArrayToString(Encoding encoding, byte[] byteArray)
        {
            try
            {
                return encoding.GetString(byteArray);
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        // Thaks to Brian Sullivan and Dzyann for their help on stackoverflow.com
        // https://stackoverflow.com/questions/12306/can-i-serialize-a-c-sharp-type-object
        private static Type GetTypeFromTypeString(string valueType)
        {
            var type = Type.GetType(valueType);
            if (type != null)
                return type;

            try
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                //To speed things up, we check first in the already loaded assemblies.
                foreach (var assembly in assemblies)
                {
                    // string assemblyName = assembly.GetName().Name;
                    type = assembly.GetType(valueType);
                    if (type != null)
                        break;
                }
                if (type != null)
                    return type;

                var loadedAssemblies = assemblies.ToList();

                foreach (var loadedAssembly in assemblies)
                {
                    foreach (AssemblyName referencedAssemblyName in loadedAssembly.GetReferencedAssemblies())
                    {
                        var found = loadedAssemblies.All(x => x.GetName() != referencedAssemblyName);

                        if (!found)
                        {
                            try
                            {
                                var referencedAssembly = Assembly.Load(referencedAssemblyName);
                                type = referencedAssembly.GetType(valueType);
                                if (type != null)
                                    break;
                                loadedAssemblies.Add(referencedAssembly);
                            }
                            catch
                            {
                                //We will ignore this, because the Type might still be in one of the other Assemblies.
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Type-generating exception: ", exception);
            }

            if (type == null)
            {
                throw new Exception("Type-generating exception: Type is null.");
            }

            return type;
        }

        #endregion private helpers
    }
}