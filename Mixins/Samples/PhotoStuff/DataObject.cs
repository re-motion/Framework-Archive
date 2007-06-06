using System;
using System.Collections.Generic;
using System.Text;

namespace Samples.PhotoStuff
{
  public class DataObject : IDataObject
  {
    public void DoSomething()
    {
      Console.WriteLine ("Doing something");
    }
  }
}
