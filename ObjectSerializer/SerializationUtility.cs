using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

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
    /// </remarks>
    public class SerializationUtility
    {
        #region public members

        #region Object Base64 string Serialization

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

        #endregion Object Base64 string Serialization

        #region Object encoded string Serialization

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

        #endregion Object encoded string Serialization

        #region Object ByteList Serialization

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

        #endregion Object ByteList Serialization

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

        #endregion private members
    }
}