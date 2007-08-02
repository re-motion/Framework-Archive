using System;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [Instantiable]
  [DBTable ("TableWithoutProperties")]
  [FirstStorageGroupAttribute]
  public abstract class ClassWithoutProperties : DomainObject
  {
  }
}