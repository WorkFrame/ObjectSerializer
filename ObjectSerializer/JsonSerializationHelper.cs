using System.Text.Json;

namespace NetEti.ObjectSerializer
{
    /// <summary>
    /// Statische Klasse mit Methoden zum Serialisieren und Deserialisieren von Objekten nach und aus Json.
    /// </summary>
    public static class JsonSerializationHelper
    {
        /// <summary>
        /// Serialisiert eine Objekt in ein Json-Format.
        /// </summary>
        /// <param name="obj">Zu serialisierendes Objekt.</param>
        /// <returns>String mit dem serialisierten Objekt.</returns>
        /// <exception cref="Exception">Mögliche Fehler bei der Serialisierung des Objekts.</exception>
        public static string SerializeToJson<T>(T obj)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                return JsonSerializer.Serialize(obj, options);
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
        /// Speichert einen JobContainer in eine Json-Datei.
        /// </summary>
        /// <param name="obj">Zu speicherndes Objekt.</param>
        /// <param name="filePath">Pfad zur Json-Datei.</param>
        /// <exception cref="Exception">Mögliche Fehler beim Speichern des Objekts.</exception>
        public static void SaveToJsonFile<T>(T obj, string filePath)
        {
            try
            {
                string jsonString = SerializeToJson<T>(obj);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving Objekt to JSON file: {filePath}", ex);
            }
        }
    }
}