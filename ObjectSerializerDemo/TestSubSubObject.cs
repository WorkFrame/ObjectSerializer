using System.Runtime.Serialization;

namespace NetEti.DemoApplications
{
    // [Serializable()]
    [DataContract()]
    public class TestSubSubObject
    {
        [DataMember()]
        public int SubSubId { get; set; }

        [DataMember()]
        public string? SubSubName { get; set; }
    }
}
