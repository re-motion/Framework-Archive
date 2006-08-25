using System;
using System.Collections.Generic;
using System.Text;

using Rubicon;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.TestDomain
{
  [PermanentGuid ("00000000-0000-0000-0002-000000000000")]
  public class PaperFile : File
  {
    private FileState _state;
    private string _location;
    
    public PaperFile ()
    {
    }

    [PermanentGuid ("00000000-0000-0000-0002-000000000001")]
    public FileState State
    {
      get { return _state; }
      set { _state = value; }
    }

    public string Location
    {
      get { return _location; }
      set { _location = value; }
    }
  }
}