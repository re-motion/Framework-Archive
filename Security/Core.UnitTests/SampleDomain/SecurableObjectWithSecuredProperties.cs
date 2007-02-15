using System;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  [AccessType]
  public enum PropertyAccessTypes
  {
    ReadSecret = 0,
    WriteSecret = 1
  }

  public class SecurableObjectWithSecuredProperties
  {
    [DemandPropertyReadPermission (PropertyAccessTypes.ReadSecret)]
    [DemandPropertyWritePermission (PropertyAccessTypes.WriteSecret)]
    public string SecretProperty
    {
      get { return string.Empty; }
      set { }
    }
  }
}
