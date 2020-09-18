using NetEti.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NetEti.DemoApplications.ObjectSerializerDemo
{
  [Serializable()]
  public class ComplexTestObject : ISerializable
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public TestSubObject SubObject { get; set; }

    public ComplexTestObject() { }

    protected ComplexTestObject(SerializationInfo info, StreamingContext context)
    {
      Id = info.GetInt32("Id");
      Name = info.GetString("Name");
      SubObject = (TestSubObject)info.GetValue("SubObject", typeof(TestSubObject));
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("Id", Id);
      info.AddValue("Name", Name);
      info.AddValue("SubObject", SubObject);
    }
  }
}
