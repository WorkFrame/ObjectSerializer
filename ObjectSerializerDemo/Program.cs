using NetEti.ObjectSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEti.DemoApplications.ObjectSerializerDemo
{
  class Program
  {
    static void Main(string[] args)
    {
      SimpleTestObject simpleTestObject = new SimpleTestObject() { Id = 1, Name = "Harry" };
      string encoded = XMLSerializationUtility.SerializeObject(Encoding.Default, simpleTestObject);
      Console.WriteLine("#" + encoded + "#");
      object obj = XMLSerializationUtility.DeserializeObject(Encoding.Default, encoded);
      Console.WriteLine(String.Format("object - Type: {0}, Id: {1}, Name: {2}", obj.GetType().Name, ((SimpleTestObject)obj).Id, ((SimpleTestObject)obj).Name));

      ComplexTestObject complexTestObject = new ComplexTestObject() { Id = 1, Name = "Harry", SubObject = new TestSubObject() { SubId = 100, SubName = "Child"} };
			encoded = XMLSerializationUtility.SerializeObject(Encoding.Default, complexTestObject);
			Console.WriteLine("#" + encoded + "#");
			obj = XMLSerializationUtility.DeserializeObject(Encoding.Default, encoded);
			Console.WriteLine(String.Format("object - Type: {0}, Id: {1}, Name: {2}\n\tSubObject.SubId: {3}, SubObject.SubName: {4}",
				obj.GetType().Name, ((ComplexTestObject)obj).Id, ((ComplexTestObject)obj).Name, ((ComplexTestObject)obj).SubObject.SubId, ((ComplexTestObject)obj).SubObject.SubName));

			encoded = XMLSerializationUtility.SerializeObjectToBase64(complexTestObject);
			Console.WriteLine("#" + encoded + "#");
			obj = XMLSerializationUtility.DeserializeObjectFromBase64(encoded);
			Console.WriteLine(String.Format("object - Type: {0}, Id: {1}, Name: {2}\n\tSubObject.SubId: {3}, SubObject.SubName: {4}",
			obj.GetType().Name, ((ComplexTestObject)obj).Id, ((ComplexTestObject)obj).Name, ((ComplexTestObject)obj).SubObject.SubId, ((ComplexTestObject)obj).SubObject.SubName));
			
			Console.ReadLine();
    }
  }
}
