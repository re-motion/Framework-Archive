using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins;

namespace Rubicon.Mixins.Samples.PhotoStuff.Variant1
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
