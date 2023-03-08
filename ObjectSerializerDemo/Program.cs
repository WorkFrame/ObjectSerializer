using NetEti.ObjectSerializer;

namespace NetEti.DemoApplications
{
    class Program
	{
		static void Main(string[] args)
		{
			SimpleTestObject simpleTestObject = new SimpleTestObject() { Id = 1, Name = "Harry" };
			string encoded = SerializationUtility.SerializeObjectToUTF8(simpleTestObject);
			Console.WriteLine("#" + encoded + "#");
			object? obj = SerializationUtility.DeserializeObjectFromUTF8<SimpleTestObject>(encoded);
            if (obj == null)
            {
                throw new InvalidOperationException("Die Deserialisierung von SimpleTestObject ist schiefgegangen!");
            }
            Console.WriteLine(String.Format("object - Type: {0}, Id: {1}, Name: {2}", obj.GetType().Name, ((SimpleTestObject)obj).Id, ((SimpleTestObject)obj).Name));

			ComplexTestObject complexTestObject = new ComplexTestObject() { Id = 1, Name = "Harry", SubObject = new TestSubObject() { SubId = 100, SubName = "Child" } };
			encoded = SerializationUtility.SerializeObjectToBase64(complexTestObject);
			Console.WriteLine("#" + encoded + "#");
			obj = SerializationUtility.DeserializeObjectFromBase64<ComplexTestObject>(encoded);
            if (obj == null)
            {
                throw new InvalidOperationException("Die Deserialisierung von ComplexTestObject ist schiefgegangen!");
            }
            TestSubObject? testSubObject = ((ComplexTestObject)obj).SubObject;
            if (testSubObject == null)
            {
                throw new InvalidOperationException("Die Deserialisierung von ComplexTestObject ist schiefgegangen!");
            }
            Console.WriteLine(String.Format("object - Type: {0}, Id: {1}, Name: {2}\n\tSubObject.SubId: {3}, SubObject.SubName: {4}",
				obj.GetType().Name, ((ComplexTestObject)obj).Id, ((ComplexTestObject)obj).Name, testSubObject.SubId, testSubObject.SubName));
			encoded = SerializationUtility.SerializeObjectToUTF8(complexTestObject);
			Console.WriteLine("#" + encoded + "#");
			obj = SerializationUtility.DeserializeObjectFromUTF8<ComplexTestObject>(encoded);
            if (obj == null)
            {
                throw new InvalidOperationException("Die Deserialisierung von ComplexTestObject ist schiefgegangen!");
            }
            testSubObject = ((ComplexTestObject)obj).SubObject;
            if (testSubObject == null)
            {
                throw new InvalidOperationException("Die Deserialisierung von ComplexTestObject ist schiefgegangen!");
            }
            Console.WriteLine(String.Format("object - Type: {0}, Id: {1}, Name: {2}\n\tSubObject.SubId: {3}, SubObject.SubName: {4}",
			obj.GetType().Name, ((ComplexTestObject)obj).Id, ((ComplexTestObject)obj).Name, testSubObject.SubId, testSubObject.SubName));

			Console.ReadLine();
		}
	}
}
