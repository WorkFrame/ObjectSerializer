using System.Runtime.Serialization;

namespace NetEti.DemoApplications
{
    /// <summary>
    /// Einfaches ReturnObject für das Testen des ObjectSerializers.
    /// </summary>
    /// <remarks>
    /// Autor: Erik Nagel, NetEti
    ///
    /// 16.03.2015 Erik Nagel: erstellt
    /// </remarks>
    [DataContract] // [Serializable()]
    public class SimpleTestObject
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string? Name { get; set; }
    }
}
