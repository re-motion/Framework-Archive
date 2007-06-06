using System;
using System.Collections.Generic;
using System.Text;
using Mixins;

namespace Samples.PhotoStuff.Variant2
{
  [Uses (typeof (DocumentMixin))]
  public class Photo
  {
    [Stored]
    public IDocument Document
    {
      get { return (IDocument) this; }
    }
  }
}
