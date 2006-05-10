using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Domain
{

  public class PaperFile : File
  {
    private string _location;
    
    public PaperFile ()
    {
    }

    public string Location
    {
      get { return _location; }
      set { _location = value; }
    }
  }
}