using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data;

namespace Rubicon.Security.UnitTests.TestDomain
{
  [PermanentGuid ("00000000-0000-0000-0001-000000000000")]
  public class File : ISecurableType
  {
    private Confidentiality _confidentiality;
    private SomeEnum _someEnum;
	
    private string _id;

    public File ()
    {
    }

    [PermanentGuid ("00000000-0000-0000-0001-000000000001")]
    public Confidentiality Confidentiality
    {
      get { return _confidentiality; }
      set { _confidentiality = value; }
    }

    public SomeEnum SimpleEnum
    {
      get { return _someEnum; }
      set { _someEnum = value; }
    }

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

   // [RequiredPermission (DomainAccessType.Journalize)]
    public void Journalize ()
    {
    }

    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}