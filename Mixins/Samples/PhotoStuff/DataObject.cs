using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.Samples.PhotoStuff
{
  public class DataObject : IDataObject
  {
    public void DoSomething()
    {
      Console.WriteLine ("Doing something");
    }
  }
}
