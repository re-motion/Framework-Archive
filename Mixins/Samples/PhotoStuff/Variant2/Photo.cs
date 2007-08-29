using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins;

namespace Rubicon.Mixins.Samples.PhotoStuff.Variant2
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
