using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Domain
{

  [PermanentGuid ("00000000-0000-0000-0002-000000000000")]
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