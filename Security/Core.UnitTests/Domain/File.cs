using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.Domain
{
  public class File : ISecurableType
  {
    private Confidentiality _confidentiality;
    private FileState _state;
    private string _id;

    public File ()
    {
    }

    public Confidentiality Confidentiality
    {
      get { return _confidentiality; }
      set { _confidentiality = value; }
    }

    public FileState State
    {
      get { return _state; }
      set { _state = value; }
    }

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }

}