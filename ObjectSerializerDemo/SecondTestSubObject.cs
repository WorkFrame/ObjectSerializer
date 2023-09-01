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

        public override string ToString()
        {
            return String.Format($"SecondTestSubObject: SubId={SubId.ToString()}, SubName={SubName}")
                + Environment.NewLine + SubSubObject?.ToString();
        }
    }
}
