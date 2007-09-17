using System;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  public class DOImplementingAbstractStorageClassNoneProperties : DOWithAbstractStorageClassNoneProperties
  {
    public override int PropertyWithGetterAndSetter
    {
      get { return 0; }
      set { Dev.Null = value; }
    }
  }
}