using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetEti.DemoApplications.ObjectSerializerDemo
{
  [Serializable()]
  public class TestSubObject
  {
    public int SubId { get; set; }
    public string SubName { get; set; }
  }
}
