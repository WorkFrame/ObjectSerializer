using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEti.DemoApplications.ObjectSerializerDemo
{
  [Serializable()]
  public class SimpleTestObject
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }
}
