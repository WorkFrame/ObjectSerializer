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

        public override string ToString()
        {
            return String.Format($"TestSubSubObject: SubSubId={SubSubId.ToString()}, SubSubName={SubSubName}");
        }
    }
}
