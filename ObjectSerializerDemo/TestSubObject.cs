using System.Runtime.Serialization;

namespace NetEti.DemoApplications
{
    // [Serializable()]
    [DataContract()]
    public class TestSubObject
    {
        [DataMember()]
        public int SubId { get; set; }

        [DataMember()]
        public string? SubName { get; set; }
    }
}
