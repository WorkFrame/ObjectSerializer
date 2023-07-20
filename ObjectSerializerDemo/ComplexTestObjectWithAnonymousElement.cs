using NetEti.ObjectSerializer;
using System.Runtime.Serialization;

namespace NetEti.DemoApplications
{
    /// <summary>
    /// ReturnObject für das Testen des ObjectSerializers.
    /// </summary>
    /// <remarks>
    /// Autor: Erik Nagel, NetEti
    ///
    /// 16.03.2015 Erik Nagel: erstellt
    /// </remarks>
    [DataContract()] // [Serializable()]
    public class ComplexTestObjectWithAnonymousElement //: ISerializable
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string? Name { get; set; }

        [AnonymousDataMember] // [DataMember]
        public object? SubObject { get; set; }

        [DataMember]
        public string? Comment { get; set; }

        [AnonymousDataMember] // [DataMember]
        public object? SecondSubObject { get; set; }

        /// <summary>
        /// Standard Konstruktor.
        /// </summary>
        public ComplexTestObjectWithAnonymousElement() { }

        /// <summary>
        /// Deserialisierungs-Konstruktor.
        /// </summary>
        /// <param name="info">Property-Container.</param>
        /// <param name="context">Übertragungs-Kontext.</param>
        protected ComplexTestObjectWithAnonymousElement(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            Name = info.GetString("Name");
            SubObject = info.GetValue("SubObject", typeof(object));
            Comment = info.GetString("Comment");
            SecondSubObject = info.GetValue("SecondSubObject", typeof(object));
        }

        /// <summary>
        /// Serialisierungs-Hilfsroutine: holt die Objekt-Properties in den Property-Container.
        /// </summary>
        /// <param name="info">Property-Container.</param>
        /// <param name="context">Serialisierungs-Kontext.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", Name);
            info.AddValue("SubObject", SubObject);
            info.AddValue("Comment", Comment);
            info.AddValue("SecondSubObject", SecondSubObject);
        }
    }
}
