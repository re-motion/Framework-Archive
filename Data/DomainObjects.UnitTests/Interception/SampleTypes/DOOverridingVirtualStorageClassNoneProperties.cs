using System;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  public class DOOverridingVirtualStorageClassNoneProperties : DOWithVirtualStorageClassNoneProperties
  {
    public override int PropertyWithGetterAndSetter
    {
      get { return 0; }
      set { Dev.Null = value; }
    }
  }
}