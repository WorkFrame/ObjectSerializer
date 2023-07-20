using NetEti.ObjectSerializer;
using System.Runtime.Serialization;

namespace NetEti.DemoApplications
{
    // [Serializable()]
    [DataContract()]
    public class SecondTestSubObject
    {
        [DataMember()]
        public int SubId { get; set; }

        [AnonymousDataMember] // [DataMember]
        public object? SubSubObject { get; set; }

        [DataMember()]
        public string? SubName { get; set; }
    }
}
