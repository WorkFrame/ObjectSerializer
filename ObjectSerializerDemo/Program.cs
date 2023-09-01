using NetEti.ObjectSerializer;

namespace NetEti.DemoApplications
{
    class Program
	{
		static void Main(string[] args)
		{
            // List<byte> encodedByteList;
            string encodedString;
            object? obj;

            ComplexTestObjectWithAnonymousElement complexTestObjectWithAnonymousElement = new ComplexTestObjectWithAnonymousElement()
            {
                Id = 1,
                Name = "Harry",
                SubObject = new TestSubObject() { SubId = 100, SubName = "Child in anonymous object" },
                Comment = "Bla Bla",
                SecondSubObject = new SecondTestSubObject() {
                    SubId = 200,
                    SubSubObject = new TestSubSubObject() { SubSubId = 1111, SubSubName = "Grandchild in Second child of anonymous object" },
                    // SubSubObject = null,
                    SubName = "Second child in anonymous object" }
            };
            // encodedByteList = SerializationUtility.SerializeObjectToByteList(complexTestObjectWithAnonymousElement);
            // Console.WriteLine($"Anzahl serialisierter Bytes: {encodedByteList.Count}");
            encodedString = SerializationUtility.SerializeObjectToBase64String(complexTestObjectWithAnonymousElement, false);
            Console.WriteLine($"Anzahl serialisierter Bytes: {encodedString.Length}");
            Console.WriteLine($"Base64-String: >{encodedString}<");
            obj = SerializationUtility.DeserializeObjectFromBase64String(encodedString);
            if (obj == null)
            {
                throw new InvalidOperationException(
                    "Die Deserialisierung von complexTestObjectWithAnonymousElement ist schiefgegangen!");
            }
            var anon = ((ComplexTestObjectWithAnonymousElement)obj).SubObject;
            if (anon is TestSubObject)
            {
                TestSubObject? anonymousSubObject = (TestSubObject?)((ComplexTestObjectWithAnonymousElement)obj).SubObject;
                if (anonymousSubObject == null)
                {
                    throw new InvalidOperationException(
                        "Die Deserialisierung von complexTestObjectWithAnonymousElement.SubObject ist schiefgegangen!");
                }
                SecondTestSubObject? SecondAnonymousSubObject
                    = (SecondTestSubObject?)((ComplexTestObjectWithAnonymousElement)obj).SecondSubObject;
                if (SecondAnonymousSubObject == null)
                {
                    throw new InvalidOperationException(
                        "Die Deserialisierung von complexTestObjectWithAnonymousElement.SecondSubObject ist schiefgegangen!");
                }
                TestSubSubObject? AnonymousSubSubObject
                    = (TestSubSubObject?)((SecondTestSubObject?)((ComplexTestObjectWithAnonymousElement)obj).SecondSubObject)?.SubSubObject;
                if (AnonymousSubSubObject == null)
                {
                    throw new InvalidOperationException(
                        "Die Deserialisierung von complexTestObjectWithAnonymousElement.SecondSubObject.SubSubObject ist schiefgegangen!");
                }
                Console.WriteLine(String.Format("object - Type: {0}, Id: {1}, Name: {2}, Comment: {3}",
                    obj.GetType().Name, ((ComplexTestObjectWithAnonymousElement)obj).Id, ((ComplexTestObjectWithAnonymousElement)obj).Name,
                    ((ComplexTestObjectWithAnonymousElement)obj).Comment));
                Console.WriteLine(String.Format("\tSubObject.SubId: {0}, SubObject.SubName: {1}",
                    anonymousSubObject.SubId, anonymousSubObject.SubName));
                Console.WriteLine(String.Format("\tSecondSubObject.SubId: {0}, SecondSubObject.SubName: {1}",
                    SecondAnonymousSubObject.SubId, SecondAnonymousSubObject.SubName));
                Console.WriteLine(String.Format("\t\tSecondSubObject.SubSubObject.SubId: {0}, SecondSubObject.SubSubObject.SubName: {1}",
                    AnonymousSubSubObject.SubSubId, AnonymousSubSubObject.SubSubName));
            }
            else
            {
                string? anonymousSubObject = (string?)((ComplexTestObjectWithAnonymousElement)obj).SubObject;
                if (anonymousSubObject == null)
                {
                    throw new InvalidOperationException(
                        "Die Deserialisierung von complexTestObjectWithAnonymousElement.SubObject ist schiefgegangen!");
                }
                string? secondAnonymousSubObject = (string?)((ComplexTestObjectWithAnonymousElement)obj).SecondSubObject;
                if (secondAnonymousSubObject == null)
                {
                    throw new InvalidOperationException(
                        "Die Deserialisierung von complexTestObjectWithAnonymousElement.SecondSubObject ist schiefgegangen!");
                }
                Console.WriteLine(String.Format("object - Type: {0}, Id: {1}, Name: {2}, Comment: {3}",
                    obj.GetType().Name, ((ComplexTestObjectWithAnonymousElement)obj).Id, ((ComplexTestObjectWithAnonymousElement)obj).Name,
                    ((ComplexTestObjectWithAnonymousElement)obj).Comment));
                Console.WriteLine(String.Format("\tSubObject: {0}", anonymousSubObject));
                Console.WriteLine(String.Format("\tSubObject2: {0}", secondAnonymousSubObject));
            }
            //-----------------------------------------------------------------------------------------------------------

            Console.ReadLine();
		}
	}
}
