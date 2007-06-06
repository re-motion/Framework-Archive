using System;
using System.Collections.Generic;
using System.Text;
using Mixins;

namespace Samples.PhotoStuff.Variant3
{
  [Uses (typeof (Document))]
  public class Photo : DataObject
  {
    [Stored]
    public IDocument Document
    {
      get { return (IDocument) this; }
    }
  }
}
