using System;
using System.Collections.Generic;
using System.Text;
using Mixins;

namespace Samples.PhotoStuff.Variant3
{
  public class Document : IDocument
  {
    private DateTime _createdAt = DateTime.Now;

    [Stored]
    public DateTime CreatedAt
    {
      get { return _createdAt; }
      set { _createdAt = value; }
    }

    public void Extend ()
    {
      Console.WriteLine ("Extending");
    }

    public void Save ()
    {
      Console.WriteLine ("Saving");
    }

    public void Print ()
    {
      Console.WriteLine ("Printing");
    }
  }
}
