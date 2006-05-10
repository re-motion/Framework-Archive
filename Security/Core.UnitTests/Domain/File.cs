using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data;

namespace Rubicon.Security.UnitTests.Domain
{
  [PermanentGuid ("00000000-0000-0000-0001-000000000000")]
  public class File : ISecurableType
  {
    private Confidentiality _confidentiality;
    private FileState _state;
    private SimpleEnum _simpleEnum;
	
    private string _id;

    public File ()
    {
    }

    [PermanentGuid ("00000000-0000-0000-0000-000000000001")]
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

    public SimpleEnum SimpleEnum
    {
      get { return _simpleEnum; }
      set { _simpleEnum = value; }
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